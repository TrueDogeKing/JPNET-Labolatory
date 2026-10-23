
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern System.Diagnostics.FileVersionInfo
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly JP_NET.Lab1a.O
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                   63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module JP_NET.Lab1a.O.dll
// MVID: {B9E36EA2-CBF7-4065-835B-CC4FA717C994}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05CB0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit JP_NET.Lab1.ClassA
       extends [System.Runtime]System.Object
{
  .field private int32 _a
  .field public int32 b
  .field public static literal char nameConst = char(0x0041)
  .field public initonly char nameReadonly
  .method public hidebysig specialname instance int32 
          get_A() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 JP_NET.Lab1.ClassA::_a
    IL_0006:  ret
  } // end of method ClassA::get_A

  .method public hidebysig specialname instance void 
          set_A(int32 'value') cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 JP_NET.Lab1.ClassA::_a
    IL_0007:  ret
  } // end of method ClassA::set_A

  .method public hidebysig instance void 
          MethodOfClassA() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class [System.Diagnostics.FileVersionInfo]System.Diagnostics.FileVersionInfo V_0,
             string V_1)
    IL_0000:  call       class [System.Runtime]System.Reflection.Assembly [System.Runtime]System.Reflection.Assembly::GetExecutingAssembly()
    IL_0005:  callvirt   instance string [System.Runtime]System.Reflection.Assembly::get_Location()
    IL_000a:  call       class [System.Diagnostics.FileVersionInfo]System.Diagnostics.FileVersionInfo [System.Diagnostics.FileVersionInfo]System.Diagnostics.FileVersionInfo::GetVersionInfo(string)
    IL_000f:  stloc.0
    IL_0010:  ldstr      "{0}.{1}"
    IL_0015:  ldloc.0
    IL_0016:  callvirt   instance int32 [System.Diagnostics.FileVersionInfo]System.Diagnostics.FileVersionInfo::get_ProductMajorPart()
    IL_001b:  box        [System.Runtime]System.Int32
    IL_0020:  ldloc.0
    IL_0021:  callvirt   instance int32 [System.Diagnostics.FileVersionInfo]System.Diagnostics.FileVersionInfo::get_ProductMinorPart()
    IL_0026:  box        [System.Runtime]System.Int32
    IL_002b:  call       string [System.Runtime]System.String::Format(string,
                                                                      object,
                                                                      object)
    IL_0030:  stloc.1
    IL_0031:  ldarg.0
    IL_0032:  ldloc.1
    IL_0033:  call       instance void JP_NET.Lab1.ClassA::PartialMethodOfClassA(object)
    IL_0038:  ret
  } // end of method ClassA::MethodOfClassA

  .method private hidebysig instance void 
          PartialMethodOfClassA(object x) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldstr      "Wydruk z klasy A. {0}"
    IL_0005:  ldarg.1
    IL_0006:  call       void [System.Console]System.Console::WriteLine(string,
                                                                        object)
    IL_000b:  ret
  } // end of method ClassA::PartialMethodOfClassA

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   65
    IL_0003:  stfld      char JP_NET.Lab1.ClassA::nameReadonly
    IL_0008:  ldarg.0
    IL_0009:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_000e:  ret
  } // end of method ClassA::.ctor

  .property instance int32 A()
  {
    .get instance int32 JP_NET.Lab1.ClassA::get_A()
    .set instance void JP_NET.Lab1.ClassA::set_A(int32)
  } // end of property ClassA::A
} // end of class JP_NET.Lab1.ClassA


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file JP_NET.Lab1a.O.res
