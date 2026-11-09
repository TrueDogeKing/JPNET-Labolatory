#pragma once
#include "DataModel.h"

using namespace System;
using namespace System::IO;
using namespace System::Windows;
using namespace System::Windows::Markup;

namespace LabCli {
    public ref class WindowFactory {
    public:
        static Window^ Create();
    };
}
