#pragma once
#include "RelayCommand.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace System::ComponentModel;
using namespace System::Reflection;
using namespace System::Windows::Input;
using namespace Microsoft::CodeAnalysis;   // tylko dla enuma OutputKind - zadnych innych typow Roslyna!

// Wlasciwosc z automatycznym powiadomieniem PropertyChanged.
#define NOTIFY_PROPERTY(type, name)                                   \
    private: type _##name;                                            \
    public: property type name {                                      \
        type get() { return _##name; }                                \
        void set(type value) { _##name = value; Notify(#name); }      \
    }

namespace LabCli {

    public ref class DataModel : public INotifyPropertyChanged {
    public:
        // "Automatyczne" (trivial) zdarzenie - kompilator generuje add/remove/raise.
        virtual event PropertyChangedEventHandler^ PropertyChanged;

        DataModel();

        // ---------- Wlasciwosci powiazane z GUI ----------
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
        // Osobno, bo po zmianie zaznaczenia trzeba pokazac typ i wartosc zmiennej.
        property int SelectedVariableIndex {
            int get() { return _selectedVariableIndex; }
            void set(int value);
        }

        property ObservableCollection<OutputKind>^ OutputKinds;
        property ObservableCollection<String^>^ Errors;
        property ObservableCollection<String^>^ Methods;
        property ObservableCollection<String^>^ Variables;

        property ICommand^ CompileCommand;
        property ICommand^ InvokeCommand;
        property ICommand^ SetCommand;

    private:
        // ---------- Stan po kompilacji ----------
        Object^ _instance;
        List<MethodInfo^>^ _methodInfos;
        List<MemberInfo^>^ _variableInfos;

        void Notify(String^ name);

        // ---------- Pomocniki refleksji ----------
        static array<Object^>^ FillDefaults(MethodBase^ method, array<Object^>^ given);
        static MethodInfo^ FindMethod(Type^ type, String^ name, Type^ firstParamType);

        // ---------- Komendy i logika ----------
        void OnCompile(Object^);
        void OnInvoke(Object^);
        void OnSet(Object^);
        void FillMethods(Type^ type);
        void FillVariables(Type^ type);
        void ShowSelectedVariable();
    };
}
