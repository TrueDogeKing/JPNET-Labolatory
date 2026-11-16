#include "pch.h"
#include "DataModel.h"

using namespace System::IO;
using namespace System::Threading;
using namespace System::Globalization;

namespace LabCli {

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

    void DataModel::Notify(String^ name) {
        PropertyChanged(this, gcnew PropertyChangedEventArgs(name));
    }

    void DataModel::SelectedVariableIndex::set(int value) {
        _selectedVariableIndex = value;
        Notify("SelectedVariableIndex");
        ShowSelectedVariable();
    }

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
                args[i] = nullptr;
        }
        return args;
    }

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


    void DataModel::OnCompile(Object^) {
        Errors->Clear();
        Methods->Clear();
        Variables->Clear();
        _methodInfos->Clear();
        _variableInfos->Clear();
        _instance = nullptr;
        Result = nullptr;

        try {
            Assembly^ caAsm = OutputKind::typeid->Assembly;
            Assembly^ vbAsm = Assembly::Load("Microsoft.CodeAnalysis.VisualBasic");
            Type^ tSyntaxTree        = caAsm->GetType("Microsoft.CodeAnalysis.SyntaxTree", true);
            Type^ tMetadataReference = caAsm->GetType("Microsoft.CodeAnalysis.MetadataReference", true);
            Type^ tCompilation       = caAsm->GetType("Microsoft.CodeAnalysis.Compilation", true);
            Type^ tVbSyntaxTree      = vbAsm->GetType("Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree", true);
            Type^ tVbCompilation     = vbAsm->GetType("Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation", true);
            Type^ tVbOptions         = vbAsm->GetType("Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilationOptions", true);

            MethodInfo^ parse = FindMethod(tVbSyntaxTree, "ParseText", String::typeid);
            Object^ tree = parse->Invoke(nullptr, FillDefaults(parse, gcnew array<Object^>{ Code }));

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

            ConstructorInfo^ ctor = nullptr;
            for each (ConstructorInfo^ c in tVbOptions->GetConstructors()) {
                array<ParameterInfo^>^ ps = c->GetParameters();
                if (ps->Length > 1 && ps[0]->ParameterType == OutputKind::typeid && ps[1]->IsOptional) {
                    ctor = c;
                    break;
                }
            }
            Object^ options = ctor->Invoke(FillDefaults(ctor, gcnew array<Object^>{ SelectedOutputKind }));

            Array^ trees = Array::CreateInstance(tSyntaxTree, 1);
            trees->SetValue(tree, 0);
            MethodInfo^ create = FindMethod(tVbCompilation, "Create", String::typeid);
            Object^ compilation = create->Invoke(nullptr, FillDefaults(create,
                gcnew array<Object^>{ gcnew String("DynamicAssembly"), trees, refs, options }));

            MemoryStream^ ms = gcnew MemoryStream();
            MethodInfo^ emit = FindMethod(tCompilation, "Emit", Stream::typeid);
            Object^ result = emit->Invoke(compilation, FillDefaults(emit, gcnew array<Object^>{ ms }));

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

            FillMethods(type);
            FillVariables(type);
        }
        catch (TargetInvocationException^ ex) {
            Errors->Add(ex->InnerException != nullptr ? ex->InnerException->ToString() : ex->ToString());
        }
        catch (Exception^ ex) {
            Errors->Add(ex->ToString());
        }
    }

    Object^ DataModel::ConvertTo(String^ text, Type^ targetType) {
        if (targetType == String::typeid)
            return text;
        if (targetType->IsEnum)
            return Enum::Parse(targetType, text);
        return Convert::ChangeType(text, targetType, CultureInfo::CurrentCulture);
    }

    void DataModel::FillMethods(Type^ type) {
        BindingFlags flags = BindingFlags::Public | BindingFlags::NonPublic
                           | BindingFlags::Instance | BindingFlags::Static
                           | BindingFlags::DeclaredOnly;
        for each (MethodInfo^ m in type->GetMethods(flags)) {
            if (m->IsSpecialName) 
                continue;
            _methodInfos->Add(m);
            Methods->Add(m->ToString());
        }
    }

    void DataModel::OnInvoke(Object^) {
        if (_instance == nullptr || SelectedMethodIndex < 0 || SelectedMethodIndex >= _methodInfos->Count) {
            Result = "Najpierw skompiluj kod i zaznacz metode.";
            return;
        }
        MethodInfo^ m = _methodInfos[SelectedMethodIndex];
        array<ParameterInfo^>^ ps = m->GetParameters();
        if (ps->Length > 1) {
            Result = "Obslugiwane sa tylko metody z 0 lub 1 parametrem.";
            return;
        }
        try {
            array<Object^>^ args = nullptr;
            if (ps->Length == 1)
                args = gcnew array<Object^>{ ConvertTo(Parameter, ps[0]->ParameterType) };

            Object^ r = m->Invoke(m->IsStatic ? nullptr : _instance, args);

            if (m->ReturnType == Void::typeid)
                Result = "(void)";
            else if (r == nullptr)
                Result = "null";
            else
                Result = r->ToString();
        }
        catch (TargetInvocationException^ ex) {
            Result = String::Format("Wyjatek w metodzie: {0}", ex->InnerException->Message);
        }
        catch (Exception^ ex) {
            Result = String::Format("Blad: {0}", ex->Message);
        }
    }

    void DataModel::FillVariables(Type^ type) {
        BindingFlags flags = BindingFlags::Public | BindingFlags::NonPublic
                           | BindingFlags::Instance | BindingFlags::Static
                           | BindingFlags::DeclaredOnly;
        for each (FieldInfo^ f in type->GetFields(flags)) {
            _variableInfos->Add(f);
            Variables->Add(String::Format("[field] {0}", f->Name));
        }
        for each (PropertyInfo^ p in type->GetProperties(flags)) {
            _variableInfos->Add(p);
            Variables->Add(String::Format("[property] {0}", p->Name));
        }
    }

    void DataModel::ShowSelectedVariable() {
        int idx = SelectedVariableIndex;
        if (_instance == nullptr || idx < 0 || idx >= _variableInfos->Count) {
            VariableType = nullptr;
            VariableValue = nullptr;
            return;
        }
        MemberInfo^ mi = _variableInfos[idx];

        FieldInfo^ f = dynamic_cast<FieldInfo^>(mi);
        if (f != nullptr) {
            VariableType = f->FieldType->FullName;
            VariableValue = Convert::ToString(f->GetValue(_instance));
            return;
        }
        PropertyInfo^ p = dynamic_cast<PropertyInfo^>(mi);
        if (p != nullptr) {
            VariableType = p->PropertyType->FullName;
            VariableValue = p->CanRead ? Convert::ToString(p->GetValue(_instance)) : nullptr;
        }
    }

    void DataModel::OnSet(Object^) {
        int idx = SelectedVariableIndex;
        if (_instance == nullptr || idx < 0 || idx >= _variableInfos->Count) {
            Result = "Najpierw skompiluj kod i zaznacz zmienna.";
            return;
        }
        MemberInfo^ mi = _variableInfos[idx];
        try {
            FieldInfo^ f = dynamic_cast<FieldInfo^>(mi);
            if (f != nullptr) {
                if (f->IsLiteral || f->IsInitOnly) {
                    Result = "To pole jest stala/ReadOnly - nie mozna go zmienic.";
                    return;
                }
                f->SetValue(_instance, ConvertTo(VariableValue, f->FieldType));
            }
            PropertyInfo^ p = dynamic_cast<PropertyInfo^>(mi);
            if (p != nullptr) {
                if (!p->CanWrite) {
                    Result = "Ta wlasciwosc nie ma settera.";
                    return;
                }
                p->SetValue(_instance, ConvertTo(VariableValue, p->PropertyType));
            }
            ShowSelectedVariable();   // odswiez wartosc odczytana z obiektu
        }
        catch (Exception^ ex) {
            Result = String::Format("Blad SET: {0}", ex->Message);
        }
    }
}
