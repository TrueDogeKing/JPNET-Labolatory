
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly JP_NET.Lab1a
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module JP_NET.Lab1a.dll
// MVID: {1217DA31-90B8-4369-BCB9-9573B0440715}
.custom instance void System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05340000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method EmbeddedAttribute::.ctor

} // end of class Microsoft.CodeAnalysis.EmbeddedAttribute

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 02 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ........T..Allow
                                                                                                                         4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                         72 69 74 65 64 00 )                               // rited.
  .field public initonly int32 Version
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_1) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  stfld      int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
    IL_000e:  ret
  } // end of method RefSafetyRulesAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RefSafetyRulesAttribute

.class public auto ansi beforefieldinit JP_NET.Lab1.ClassA
       extends [mscorlib]System.Object
{
  .field private int32 _a
  .field public int32 b
  .field public static literal char nameConst = char(0x0042)
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
    // Code size       61 (0x3d)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.Assembly V_0,
             class [System]System.Diagnostics.FileVersionInfo V_1,
             string V_2)
    IL_0000:  nop
    IL_0001:  call       class [mscorlib]System.Reflection.Assembly [mscorlib]System.Reflection.Assembly::GetExecutingAssembly()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
    IL_000d:  call       class [System]System.Diagnostics.FileVersionInfo [System]System.Diagnostics.FileVersionInfo::GetVersionInfo(string)
    IL_0012:  stloc.1
    IL_0013:  ldstr      "{0}.{1}"
    IL_0018:  ldloc.1
    IL_0019:  callvirt   instance int32 [System]System.Diagnostics.FileVersionInfo::get_ProductMajorPart()
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  ldloc.1
    IL_0024:  callvirt   instance int32 [System]System.Diagnostics.FileVersionInfo::get_ProductMinorPart()
    IL_0029:  box        [mscorlib]System.Int32
    IL_002e:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0033:  stloc.2
    IL_0034:  ldarg.0
    IL_0035:  ldloc.2
    IL_0036:  call       instance void JP_NET.Lab1.ClassA::PartialMethodOfClassA(object)
    IL_003b:  nop
    IL_003c:  ret
  } // end of method ClassA::MethodOfClassA

  .method private hidebysig instance void 
          PartialMethodOfClassA(object x) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "Wydruk z klasy A po spatchowaniu CIL. {0}"
    IL_0006:  ldarg.1
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_000c:  nop
    IL_000d:  ret
  } // end of method ClassA::PartialMethodOfClassA

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   66
    IL_0003:  stfld      char JP_NET.Lab1.ClassA::nameReadonly
    IL_0008:  ldarg.0
    IL_0009:  call       instance void [mscorlib]System.Object::.ctor()
    IL_000e:  nop
    IL_000f:  ret
  } // end of method ClassA::.ctor

  .property instance int32 A()
  {
    .get instance int32 JP_NET.Lab1.ClassA::get_A()
    .set instance void JP_NET.Lab1.ClassA::set_A(int32)
  } // end of property ClassA::A
} // end of class JP_NET.Lab1.ClassA


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file JP_NET.Lab1a.res
