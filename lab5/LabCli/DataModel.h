#pragma once
#include "RelayCommand.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace System::ComponentModel;
using namespace System::Reflection;
using namespace System::Windows::Input;
using namespace Microsoft::CodeAnalysis;

#define NOTIFY_PROPERTY(type, name)                                   \
    private: type _##name;                                            \
    public: property type name {                                      \
        type get() { return _##name; }                                \
        void set(type value) { _##name = value; Notify(#name); }      \
    }

namespace LabCli {

    public ref class DataModel : public INotifyPropertyChanged {
    public:
        virtual event PropertyChangedEventHandler^ PropertyChanged;

    private:
        void Notify(String^ name) {
            PropertyChanged(this, gcnew PropertyChangedEventArgs(name));
        }

        NOTIFY_PROPERTY(String^, Code)
        NOTIFY_PROPERTY(OutputKind, SelectedOutputKind)
        NOTIFY_PROPERTY(int, SelectedMethodIndex)
        NOTIFY_PROPERTY(String^, Parameter)
        NOTIFY_PROPERTY(String^, Result)
        NOTIFY_PROPERTY(String^, VariableType)
        NOTIFY_PROPERTY(String^, VariableValue)

    private:
        int _selectedVariableIndex = -1;
    public:
        property int SelectedVariableIndex {
            int get() { return _selectedVariableIndex; }
            void set(int value) {
                _selectedVariableIndex = value;
                Notify("SelectedVariableIndex");
                ShowSelectedVariable();
            }
        }

        property ObservableCollection<OutputKind>^ OutputKinds;
        property ObservableCollection<String^>^ Errors;
        property ObservableCollection<String^>^ Methods;
        property ObservableCollection<String^>^ Variables;

        property ICommand^ CompileCommand;
        property ICommand^ InvokeCommand;
        property ICommand^ SetCommand;

        DataModel() {
            OutputKinds = gcnew ObservableCollection<OutputKind>();
            for each (OutputKind k in Enum::GetValues(OutputKind::typeid))
                OutputKinds->Add(k);
            SelectedOutputKind = OutputKind::DynamicallyLinkedLibrary;

            Errors = gcnew ObservableCollection<String^>();
            Methods = gcnew ObservableCollection<String^>();
            Variables = gcnew ObservableCollection<String^>();
            SelectedMethodIndex = -1;

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

    private:
        Object^ _instance;
        List<MethodInfo^>^ _methodInfos = gcnew List<MethodInfo^>();
        List<MemberInfo^>^ _variableInfos = gcnew List<MemberInfo^>();

        void OnCompile(Object^) {
            Errors->Clear();
            Errors->Add(String::Format("TODO: kompilacja Roslyn (OutputKind = {0})", SelectedOutputKind));
            Methods->Clear();
            Methods->Add("TODO: metoda 1");
            Methods->Add("TODO: metoda 2");
            Variables->Clear();
            Variables->Add("TODO: zmienna 1");
        }

        void OnInvoke(Object^) {
            Result = String::Format("INVOKE: metoda #{0}, parametr = {1}", SelectedMethodIndex, Parameter);
        }

        void OnSet(Object^) {
            Result = String::Format("SET: zmienna #{0} = {1}", SelectedVariableIndex, VariableValue);
        }

        void ShowSelectedVariable() {
            if (SelectedVariableIndex >= 0)
                VariableType = "TODO: typ";
            else
                VariableType = nullptr;
            VariableValue = nullptr;
        }
    };
}
