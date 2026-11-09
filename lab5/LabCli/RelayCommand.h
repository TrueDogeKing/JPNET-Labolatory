#pragma once

using namespace System;
using namespace System::Windows::Input;

namespace LabCli {

    // Prosta implementacja ICommand - deleguje Execute do przekazanej metody.
    public ref class RelayCommand : public ICommand {
        Action<Object^>^ _execute;
    public:
        RelayCommand(Action<Object^>^ execute) : _execute(execute) {}

        virtual event EventHandler^ CanExecuteChanged;
        virtual bool CanExecute(Object^) { return true; }
        virtual void Execute(Object^ parameter) { _execute(parameter); }
    };
}
