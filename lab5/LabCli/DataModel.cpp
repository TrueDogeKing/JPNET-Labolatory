#include "pch.h"
#include "DataModel.h"

using namespace System::IO;
using namespace System::Threading;

namespace LabCli {

    // ================= Konstruktor =================

    DataModel::DataModel() {
        OutputKinds = gcnew ObservableCollection<OutputKind>();
        for each (OutputKind k in Enum::GetValues(OutputKind::typeid))
            OutputKinds->Add(k);
        SelectedOutputKind = OutputKind::DynamicallyLinkedLibrary;

        Errors = gcnew ObservableCollection<String^>();
        Methods = gcnew ObservableCollection<String^>();
        Variables = gcnew ObservableCollection<String^>();
        SelectedMethodIndex = -1;

        _methodInfos = gcnew List<MethodInfo^>();
        _variableInfos = gcnew List<MemberInfo^>();

        CompileCommand = gcnew RelayCommand(gcnew Action<Object^>(this, &DataModel::OnCompile));
        InvokeCommand  = gcnew RelayCommand(gcnew Action<Object^>(this, &DataModel::OnInvoke));
        SetCommand     = gcnew RelayCommand(gcnew Action<Object^>(this, &DataModel::OnSet));

        Code = gcnew String(R"VB(Public Class Calculator
    Private counter As Integer = 0
    Public Factor As Double = 2.5
    Public Property Name As String = "Calc"

    Public Function Multiply(x As Double) As Double
        counter += 1
        Return x * Factor
    End Function

    Public Function Describe() As String
        Return Name & ": called " & counter & " times"
    End Function
End Class
)VB");
    }

    // ================= INotifyPropertyChanged =================

    void DataModel::Notify(String^ name) {
        PropertyChanged(this, gcnew PropertyChangedEventArgs(name));
    }

    void DataModel::SelectedVariableIndex::set(int value) {
        _selectedVariableIndex = value;
        Notify("SelectedVariableIndex");
        ShowSelectedVariable();
    }

    // ================= Pomocniki refleksji =================

    // C++/CLI nie obsluguje parametrow opcjonalnych z C#.
    // Dopelnia brakujace argumenty wartosciami domyslnymi, zeby wywolac metode/konstruktor refleksja.
    array<Object^>^ DataModel::FillDefaults(MethodBase^ method, array<Object^>^ given) {
        array<ParameterInfo^>^ ps = method->GetParameters();
        array<Object^>^ args = gcnew array<Object^>(ps->Length);
        for (int i = 0; i < ps->Length; i++) {
            if (i < given->Length)
                args[i] = given[i];
            else if (ps[i]->HasDefaultValue && ps[i]->DefaultValue != nullptr) {
                args[i] = ps[i]->DefaultValue;
                if (ps[i]->ParameterType->IsEnum)
                    args[i] = Enum::ToObject(ps[i]->ParameterType, args[i]);
            }
            else
                args[i] = nullptr;   // null dla struktury = default(T)
        }
        return args;
    }

    // Szuka publicznej metody o danej nazwie, ktorej 1. parametr ma podany typ,
    // a wszystkie pozostale sa opcjonalne. Przy kilku pasujacych bierze te z najmniejsza liczba parametrow.
    MethodInfo^ DataModel::FindMethod(Type^ type, String^ name, Type^ firstParamType) {
        MethodInfo^ best = nullptr;
        for each (MethodInfo^ m in type->GetMethods()) {
            if (!m->Name->Equals(name)) continue;
            array<ParameterInfo^>^ ps = m->GetParameters();
            if (ps->Length == 0 || ps[0]->ParameterType != firstParamType) continue;
            bool restOptional = true;
            for (int i = 1; i < ps->Length; i++)
                if (!ps[i]->IsOptional) { restOptional = false; break; }
            if (!restOptional) continue;
            if (best == nullptr || ps->Length < best->GetParameters()->Length)
                best = m;
        }
        return best;
    }

    // ================= Czesc 2: kompilacja Roslyn =================

    void DataModel::OnCompile(Object^) {
        // 2.2: czyscimy wszystko przed nowa kompilacja
        Errors->Clear();
        Methods->Clear();
        Variables->Clear();
        _methodInfos->Clear();
        _variableInfos->Clear();
        _instance = nullptr;
        Result = nullptr;

        // Wszystkie wywolania Roslyna ida przez refleksje, a obiekty trzymamy jako Object^.
        // Dzieki temu cl.exe nie importuje sygnatur z System.Collections.Immutable (bledy importu metadanych).
        try {
            // Typy Roslyna pobieramy po nazwie w runtime - w kodzie C++ nie moze pojawic sie
            // zaden typ Roslyna (poza enumem OutputKind), bo cl.exe wywraca sie na ich metadanych.
            Assembly^ caAsm = OutputKind::typeid->Assembly;   // Microsoft.CodeAnalysis.dll
            Assembly^ vbAsm = Assembly::Load("Microsoft.CodeAnalysis.VisualBasic");
            Type^ tSyntaxTree        = caAsm->GetType("Microsoft.CodeAnalysis.SyntaxTree", true);
            Type^ tMetadataReference = caAsm->GetType("Microsoft.CodeAnalysis.MetadataReference", true);
            Type^ tCompilation       = caAsm->GetType("Microsoft.CodeAnalysis.Compilation", true);
            Type^ tVbSyntaxTree      = vbAsm->GetType("Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree", true);
            Type^ tVbCompilation     = vbAsm->GetType("Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation", true);
            Type^ tVbOptions         = vbAsm->GetType("Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilationOptions", true);

            // --- 2.1a: SyntaxTree (VisualBasicSyntaxTree.ParseText) ---
            MethodInfo^ parse = FindMethod(tVbSyntaxTree, "ParseText", String::typeid);
            Object^ tree = parse->Invoke(nullptr, FillDefaults(parse, gcnew array<Object^>{ Code }));

            // --- 2.1b: referencje do metadanych (List<MetadataReference>) ---
            MethodInfo^ createRef = FindMethod(tMetadataReference, "CreateFromFile", String::typeid);
            Type^ refListType = List<Object^>::typeid->GetGenericTypeDefinition()
                                    ->MakeGenericType(tMetadataReference);
            System::Collections::IList^ refs =
                safe_cast<System::Collections::IList^>(Activator::CreateInstance(refListType));

            String^ tpa = safe_cast<String^>(AppContext::GetData("TRUSTED_PLATFORM_ASSEMBLIES"));
            for each (String^ path in tpa->Split(Path::PathSeparator)) {
                String^ name = Path::GetFileName(path);
                if (name->StartsWith("System.") || name->StartsWith("Microsoft.VisualBasic")
                    || name->Equals("netstandard.dll") || name->Equals("mscorlib.dll")
                    || name->Equals("System.dll"))
                    refs->Add(createRef->Invoke(nullptr, FillDefaults(createRef, gcnew array<Object^>{ path })));
            }

            // --- 2.1c: VisualBasicCompilationOptions(OutputKind z ComboBoxa) ---
            ConstructorInfo^ ctor = nullptr;
            for each (ConstructorInfo^ c in tVbOptions->GetConstructors()) {
                array<ParameterInfo^>^ ps = c->GetParameters();
                if (ps->Length > 1 && ps[0]->ParameterType == OutputKind::typeid && ps[1]->IsOptional) {
                    ctor = c;
                    break;
                }
            }
            Object^ options = ctor->Invoke(FillDefaults(ctor, gcnew array<Object^>{ SelectedOutputKind }));

            // --- 2.1d: VisualBasicCompilation.Create(...) ---
            Array^ trees = Array::CreateInstance(tSyntaxTree, 1);
            trees->SetValue(tree, 0);
            MethodInfo^ create = FindMethod(tVbCompilation, "Create", String::typeid);
            Object^ compilation = create->Invoke(nullptr, FillDefaults(create,
                gcnew array<Object^>{ gcnew String("DynamicAssembly"), trees, refs, options }));

            // --- 2.1e: Emit do MemoryStream ---
            MemoryStream^ ms = gcnew MemoryStream();
            MethodInfo^ emit = FindMethod(tCompilation, "Emit", Stream::typeid);
            Object^ result = emit->Invoke(compilation, FillDefaults(emit, gcnew array<Object^>{ ms }));

            // --- 2.2: bledy -> Errors (Success i Diagnostics czytane refleksja) ---
            bool success = safe_cast<bool>(result->GetType()->GetProperty("Success")->GetValue(result));
            if (!success) {
                Object^ diags = result->GetType()->GetProperty("Diagnostics")->GetValue(result);
                for each (Object^ d in safe_cast<System::Collections::IEnumerable^>(diags)) {
                    Object^ severity = d->GetType()->GetProperty("Severity")->GetValue(d);
                    if (severity->ToString()->Equals("Error"))
                        Errors->Add(d->ToString());
                }
                return;
            }

            // --- 2.2: sukces -> ladujemy zestaw i tworzymy instancje ---
            Assembly^ assembly = Assembly::Load(ms->ToArray());
            Type^ type = nullptr;
            for each (Type^ t in assembly->GetTypes())
                if (t->IsClass && t->IsPublic && !t->IsAbstract) {
                    type = t;
                    break;
                }
            if (type == nullptr) {
                Errors->Add("Brak publicznej klasy w kodzie.");
                return;
            }
            _instance = Activator::CreateInstance(type);

            FillMethods(type);     // czesc 3
            FillVariables(type);   // czesc 4

            Result = String::Format("Utworzono instancje: {0}", type->FullName); // tymczasowo, do testu
        }
        catch (TargetInvocationException^ ex) {
            Errors->Add(ex->InnerException != nullptr ? ex->InnerException->ToString() : ex->ToString());
        }
        catch (Exception^ ex) {
            Errors->Add(ex->ToString());
        }
    }

    // ================= Czesc 3: metody =================

    void DataModel::FillMethods(Type^ type) {
        // czesc 3 - uzupelnimy w nastepnym kroku
    }

    void DataModel::OnInvoke(Object^) {
        Result = String::Format("INVOKE: metoda #{0}, parametr = {1}", SelectedMethodIndex, Parameter);
    }

    // ================= Czesc 4: pola i wlasciwosci =================

    void DataModel::FillVariables(Type^ type) {
        // czesc 4 - uzupelnimy pozniej
    }

    void DataModel::ShowSelectedVariable() {
        if (SelectedVariableIndex >= 0)
            VariableType = "TODO: typ";
        else
            VariableType = nullptr;
        VariableValue = nullptr;
    }

    void DataModel::OnSet(Object^) {
        Result = String::Format("SET: zmienna #{0} = {1}", SelectedVariableIndex, VariableValue);
    }
}
