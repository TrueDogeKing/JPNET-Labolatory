
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 8:0:0:0
}
.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 8:0:0:0
}
.assembly extern System.Memory
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 8:0:0:0
}
.assembly Task7
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                   63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 18 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56   // ....NETCoreApp,V
                                                                                                              65 72 73 69 6F 6E 3D 76 38 2E 30 01 00 54 0E 14   // ersion=v8.0..T..
                                                                                                              46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79   // FrameworkDisplay
                                                                                                              4E 61 6D 65 08 2E 4E 45 54 20 38 2E 30 )          // Name..NET 8.0
  .custom instance void [System.Runtime]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 05 54 61 73 6B 37 00 00 )                   // ...Task7..
  .custom instance void [System.Runtime]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 07 52 65 6C 65 61 73 65 00 00 )             // ...Release..
  .custom instance void [System.Runtime]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [System.Runtime]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 30 2E 30 00 00 )                   // ...1.0.0..
  .custom instance void [System.Runtime]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 05 54 61 73 6B 37 00 00 )                   // ...Task7..
  .custom instance void [System.Runtime]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 05 54 61 73 6B 37 00 00 )                   // ...Task7..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module Task7.dll
// MVID: {DFFAE23E-B831-4516-98C1-32EE897BB095}
.custom instance void [System.Runtime]System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06A10000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit Program
       extends [System.Runtime]System.Object
{
  .method private hidebysig static void  Main() cil managed
  {
    .entrypoint
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  call       void Program::HeavyMethod()
    IL_0005:  call       void SpanDemo::Run()
    IL_000a:  ret
  } // end of method Program::Main

  .method private hidebysig static void  HeavyMethod() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  3
    .locals init (float64 V_0,
             int32 V_1)
    IL_0000:  ldc.r8     0.0
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_0024

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  conv.r8
    IL_0011:  call       float64 [System.Runtime]System.Math::Sqrt(float64)
    IL_0016:  ldloc.1
    IL_0017:  conv.r8
    IL_0018:  call       float64 [System.Runtime]System.Math::Sin(float64)
    IL_001d:  mul
    IL_001e:  add
    IL_001f:  stloc.0
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4     0xc350
    IL_002a:  blt.s      IL_000e

    IL_002c:  ldloc.0
    IL_002d:  call       void [System.Console]System.Console::WriteLine(float64)
    IL_0032:  ret
  } // end of method Program::HeavyMethod

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Program::.ctor

} // end of class Program

.class private abstract auto ansi sealed beforefieldinit SpanDemo
       extends [System.Runtime]System.Object
{
  .field private static literal string Sample = "STATUS=200;ID=49201;LINE=12;DELAY=00:04:15"
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .method public hidebysig static valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> 
          ParseClassic(string s) cil managed
  {
    .param [0]
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 02 00 00 00 04 6C 69 6E 65 05 64 65 6C 61   // .......line.dela
                                                                                                                          79 00 00 )                                        // y..
    .param [1]
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    // Code size       123 (0x7b)
    .maxstack  4
    .locals init (int32 V_0,
             valuetype [System.Runtime]System.TimeSpan V_1,
             string[] V_2,
             int32 V_3,
             int32 V_4,
             string V_5,
             string V_6)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_1
    IL_0004:  initobj    [System.Runtime]System.TimeSpan
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.s   59
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance string[] [System.Runtime]System.String::Split(char,
                                                                                valuetype [System.Runtime]System.StringSplitOptions)
    IL_0013:  stloc.2
    IL_0014:  ldc.i4.0
    IL_0015:  stloc.3
    IL_0016:  br.s       IL_006d

    IL_0018:  ldloc.2
    IL_0019:  ldloc.3
    IL_001a:  ldelem.ref
    IL_001b:  dup
    IL_001c:  ldc.i4.s   61
    IL_001e:  callvirt   instance int32 [System.Runtime]System.String::IndexOf(char)
    IL_0023:  stloc.s    V_4
    IL_0025:  dup
    IL_0026:  ldc.i4.0
    IL_0027:  ldloc.s    V_4
    IL_0029:  callvirt   instance string [System.Runtime]System.String::Substring(int32,
                                                                                  int32)
    IL_002e:  stloc.s    V_5
    IL_0030:  ldloc.s    V_4
    IL_0032:  ldc.i4.1
    IL_0033:  add
    IL_0034:  callvirt   instance string [System.Runtime]System.String::Substring(int32)
    IL_0039:  stloc.s    V_6
    IL_003b:  ldloc.s    V_5
    IL_003d:  ldstr      "LINE"
    IL_0042:  call       bool [System.Runtime]System.String::op_Equality(string,
                                                                         string)
    IL_0047:  brfalse.s  IL_0053

    IL_0049:  ldloc.s    V_6
    IL_004b:  call       int32 [System.Runtime]System.Int32::Parse(string)
    IL_0050:  stloc.0
    IL_0051:  br.s       IL_0069

    IL_0053:  ldloc.s    V_5
    IL_0055:  ldstr      "DELAY"
    IL_005a:  call       bool [System.Runtime]System.String::op_Equality(string,
                                                                         string)
    IL_005f:  brfalse.s  IL_0069

    IL_0061:  ldloc.s    V_6
    IL_0063:  call       valuetype [System.Runtime]System.TimeSpan [System.Runtime]System.TimeSpan::Parse(string)
    IL_0068:  stloc.1
    IL_0069:  ldloc.3
    IL_006a:  ldc.i4.1
    IL_006b:  add
    IL_006c:  stloc.3
    IL_006d:  ldloc.3
    IL_006e:  ldloc.2
    IL_006f:  ldlen
    IL_0070:  conv.i4
    IL_0071:  blt.s      IL_0018

    IL_0073:  ldloc.0
    IL_0074:  ldloc.1
    IL_0075:  newobj     instance void valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan>::.ctor(!0,
                                                                                                                                             !1)
    IL_007a:  ret
  } // end of method SpanDemo::ParseClassic

  .method public hidebysig static valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> 
          ParseSpan(valuetype [System.Runtime]System.ReadOnlySpan`1<char> s) cil managed
  {
    .param [0]
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 02 00 00 00 04 6C 69 6E 65 05 64 65 6C 61   // .......line.dela
                                                                                                                          79 00 00 )                                        // y..
    // Code size       178 (0xb2)
    .maxstack  3
    .locals init (int32 V_0,
             valuetype [System.Runtime]System.TimeSpan V_1,
             int32 V_2,
             valuetype [System.Runtime]System.ReadOnlySpan`1<char> V_3,
             int32 V_4,
             valuetype [System.Runtime]System.ReadOnlySpan`1<char> V_5,
             valuetype [System.Runtime]System.ReadOnlySpan`1<char> V_6)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_1
    IL_0004:  initobj    [System.Runtime]System.TimeSpan
    IL_000a:  br         IL_009e

    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.s   59
    IL_0012:  call       int32 [System.Memory]System.MemoryExtensions::IndexOf<char>(valuetype [System.Runtime]System.ReadOnlySpan`1<!!0>,
                                                                                     !!0)
    IL_0017:  stloc.2
    IL_0018:  ldloc.2
    IL_0019:  ldc.i4.0
    IL_001a:  bge.s      IL_001f

    IL_001c:  ldarg.0
    IL_001d:  br.s       IL_0028

    IL_001f:  ldarga.s   s
    IL_0021:  ldc.i4.0
    IL_0022:  ldloc.2
    IL_0023:  call       instance valuetype [System.Runtime]System.ReadOnlySpan`1<!0> valuetype [System.Runtime]System.ReadOnlySpan`1<char>::Slice(int32,
                                                                                                                                                   int32)
    IL_0028:  stloc.3
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.0
    IL_002b:  bge.s      IL_0034

    IL_002d:  call       valuetype [System.Runtime]System.ReadOnlySpan`1<!0> valuetype [System.Runtime]System.ReadOnlySpan`1<char>::get_Empty()
    IL_0032:  br.s       IL_003e

    IL_0034:  ldarga.s   s
    IL_0036:  ldloc.2
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  call       instance valuetype [System.Runtime]System.ReadOnlySpan`1<!0> valuetype [System.Runtime]System.ReadOnlySpan`1<char>::Slice(int32)
    IL_003e:  starg.s    s
    IL_0040:  ldloc.3
    IL_0041:  ldc.i4.s   61
    IL_0043:  call       int32 [System.Memory]System.MemoryExtensions::IndexOf<char>(valuetype [System.Runtime]System.ReadOnlySpan`1<!!0>,
                                                                                     !!0)
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloca.s   V_3
    IL_004c:  ldc.i4.0
    IL_004d:  ldloc.s    V_4
    IL_004f:  call       instance valuetype [System.Runtime]System.ReadOnlySpan`1<!0> valuetype [System.Runtime]System.ReadOnlySpan`1<char>::Slice(int32,
                                                                                                                                                   int32)
    IL_0054:  stloc.s    V_5
    IL_0056:  ldloca.s   V_3
    IL_0058:  ldloc.s    V_4
    IL_005a:  ldc.i4.1
    IL_005b:  add
    IL_005c:  call       instance valuetype [System.Runtime]System.ReadOnlySpan`1<!0> valuetype [System.Runtime]System.ReadOnlySpan`1<char>::Slice(int32)
    IL_0061:  stloc.s    V_6
    IL_0063:  ldloc.s    V_5
    IL_0065:  ldstr      "LINE"
    IL_006a:  call       valuetype [System.Runtime]System.ReadOnlySpan`1<char> [System.Memory]System.MemoryExtensions::AsSpan(string)
    IL_006f:  call       bool [System.Memory]System.MemoryExtensions::SequenceEqual<char>(valuetype [System.Runtime]System.ReadOnlySpan`1<!!0>,
                                                                                          valuetype [System.Runtime]System.ReadOnlySpan`1<!!0>)
    IL_0074:  brfalse.s  IL_0082

    IL_0076:  ldloc.s    V_6
    IL_0078:  ldc.i4.7
    IL_0079:  ldnull
    IL_007a:  call       int32 [System.Runtime]System.Int32::Parse(valuetype [System.Runtime]System.ReadOnlySpan`1<char>,
                                                                   valuetype [System.Runtime]System.Globalization.NumberStyles,
                                                                   class [System.Runtime]System.IFormatProvider)
    IL_007f:  stloc.0
    IL_0080:  br.s       IL_009e

    IL_0082:  ldloc.s    V_5
    IL_0084:  ldstr      "DELAY"
    IL_0089:  call       valuetype [System.Runtime]System.ReadOnlySpan`1<char> [System.Memory]System.MemoryExtensions::AsSpan(string)
    IL_008e:  call       bool [System.Memory]System.MemoryExtensions::SequenceEqual<char>(valuetype [System.Runtime]System.ReadOnlySpan`1<!!0>,
                                                                                          valuetype [System.Runtime]System.ReadOnlySpan`1<!!0>)
    IL_0093:  brfalse.s  IL_009e

    IL_0095:  ldloc.s    V_6
    IL_0097:  ldnull
    IL_0098:  call       valuetype [System.Runtime]System.TimeSpan [System.Runtime]System.TimeSpan::Parse(valuetype [System.Runtime]System.ReadOnlySpan`1<char>,
                                                                                                          class [System.Runtime]System.IFormatProvider)
    IL_009d:  stloc.1
    IL_009e:  ldarga.s   s
    IL_00a0:  call       instance bool valuetype [System.Runtime]System.ReadOnlySpan`1<char>::get_IsEmpty()
    IL_00a5:  brfalse    IL_000f

    IL_00aa:  ldloc.0
    IL_00ab:  ldloc.1
    IL_00ac:  newobj     instance void valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan>::.ctor(!0,
                                                                                                                                             !1)
    IL_00b1:  ret
  } // end of method SpanDemo::ParseSpan

  .method public hidebysig static void  Run() cil managed
  {
    // Code size       280 (0x118)
    .maxstack  4
    .locals init (int32 V_0,
             int64 V_1,
             int64 V_2,
             int64 V_3,
             valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> V_4,
             int32 V_5,
             int32 V_6,
             int64 V_7)
    IL_0000:  ldstr      "STATUS=200;ID=49201;LINE=12;DELAY=00:04:15"
    IL_0005:  call       valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> SpanDemo::ParseClassic(string)
    IL_000a:  pop
    IL_000b:  ldstr      "STATUS=200;ID=49201;LINE=12;DELAY=00:04:15"
    IL_0010:  call       valuetype [System.Runtime]System.ReadOnlySpan`1<char> [System.Runtime]System.String::op_Implicit(string)
    IL_0015:  call       valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> SpanDemo::ParseSpan(valuetype [System.Runtime]System.ReadOnlySpan`1<char>)
    IL_001a:  pop
    IL_001b:  ldc.i4.0
    IL_001c:  stloc.0
    IL_001d:  call       int64 [System.Runtime]System.GC::GetAllocatedBytesForCurrentThread()
    IL_0022:  stloc.1
    IL_0023:  ldc.i4.0
    IL_0024:  stloc.s    V_5
    IL_0026:  br.s       IL_0040

    IL_0028:  ldloc.0
    IL_0029:  ldstr      "STATUS=200;ID=49201;LINE=12;DELAY=00:04:15"
    IL_002e:  call       valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> SpanDemo::ParseClassic(string)
    IL_0033:  ldfld      !0 valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan>::Item1
    IL_0038:  add
    IL_0039:  stloc.0
    IL_003a:  ldloc.s    V_5
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  stloc.s    V_5
    IL_0040:  ldloc.s    V_5
    IL_0042:  ldc.i4     0x3e8
    IL_0047:  blt.s      IL_0028

    IL_0049:  call       int64 [System.Runtime]System.GC::GetAllocatedBytesForCurrentThread()
    IL_004e:  ldloc.1
    IL_004f:  sub
    IL_0050:  stloc.2
    IL_0051:  call       int64 [System.Runtime]System.GC::GetAllocatedBytesForCurrentThread()
    IL_0056:  stloc.1
    IL_0057:  ldc.i4.0
    IL_0058:  stloc.s    V_6
    IL_005a:  br.s       IL_0079

    IL_005c:  ldloc.0
    IL_005d:  ldstr      "STATUS=200;ID=49201;LINE=12;DELAY=00:04:15"
    IL_0062:  call       valuetype [System.Runtime]System.ReadOnlySpan`1<char> [System.Runtime]System.String::op_Implicit(string)
    IL_0067:  call       valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> SpanDemo::ParseSpan(valuetype [System.Runtime]System.ReadOnlySpan`1<char>)
    IL_006c:  ldfld      !0 valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan>::Item1
    IL_0071:  add
    IL_0072:  stloc.0
    IL_0073:  ldloc.s    V_6
    IL_0075:  ldc.i4.1
    IL_0076:  add
    IL_0077:  stloc.s    V_6
    IL_0079:  ldloc.s    V_6
    IL_007b:  ldc.i4     0x3e8
    IL_0080:  blt.s      IL_005c

    IL_0082:  call       int64 [System.Runtime]System.GC::GetAllocatedBytesForCurrentThread()
    IL_0087:  ldloc.1
    IL_0088:  sub
    IL_0089:  stloc.3
    IL_008a:  ldstr      "STATUS=200;ID=49201;LINE=12;DELAY=00:04:15"
    IL_008f:  call       valuetype [System.Runtime]System.ReadOnlySpan`1<char> [System.Runtime]System.String::op_Implicit(string)
    IL_0094:  call       valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan> SpanDemo::ParseSpan(valuetype [System.Runtime]System.ReadOnlySpan`1<char>)
    IL_0099:  stloc.s    V_4
    IL_009b:  ldstr      "LINE="
    IL_00a0:  ldloca.s   V_4
    IL_00a2:  ldflda     !0 valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan>::Item1
    IL_00a7:  call       instance string [System.Runtime]System.Int32::ToString()
    IL_00ac:  ldstr      ", DELAY="
    IL_00b1:  ldloca.s   V_4
    IL_00b3:  ldflda     !1 valuetype [System.Runtime]System.ValueTuple`2<int32,valuetype [System.Runtime]System.TimeSpan>::Item2
    IL_00b8:  constrained. [System.Runtime]System.TimeSpan
    IL_00be:  callvirt   instance string [System.Runtime]System.Object::ToString()
    IL_00c3:  call       string [System.Runtime]System.String::Concat(string,
                                                                      string,
                                                                      string,
                                                                      string)
    IL_00c8:  call       void [System.Console]System.Console::WriteLine(string)
    IL_00cd:  ldstr      "Classic: "
    IL_00d2:  ldloc.2
    IL_00d3:  ldc.i4     0x3e8
    IL_00d8:  conv.i8
    IL_00d9:  div
    IL_00da:  stloc.s    V_7
    IL_00dc:  ldloca.s   V_7
    IL_00de:  call       instance string [System.Runtime]System.Int64::ToString()
    IL_00e3:  ldstr      " B na wywolanie"
    IL_00e8:  call       string [System.Runtime]System.String::Concat(string,
                                                                      string,
                                                                      string)
    IL_00ed:  call       void [System.Console]System.Console::WriteLine(string)
    IL_00f2:  ldstr      "Span:    "
    IL_00f7:  ldloc.3
    IL_00f8:  ldc.i4     0x3e8
    IL_00fd:  conv.i8
    IL_00fe:  div
    IL_00ff:  stloc.s    V_7
    IL_0101:  ldloca.s   V_7
    IL_0103:  call       instance string [System.Runtime]System.Int64::ToString()
    IL_0108:  ldstr      " B na wywolanie"
    IL_010d:  call       string [System.Runtime]System.String::Concat(string,
                                                                      string,
                                                                      string)
    IL_0112:  call       void [System.Console]System.Console::WriteLine(string)
    IL_0117:  ret
  } // end of method SpanDemo::Run

} // end of class SpanDemo


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file Task7.res
