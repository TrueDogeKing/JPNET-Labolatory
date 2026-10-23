using Autofac;
using Autofac.Configuration;
using Autofac.Core;
using LabIoC.Core;
using Microsoft.Extensions.Configuration;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// --- Część 7: wczytanie konfiguracji z pliku JSON ---
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var builder = new ContainerBuilder();

// Część 7: Logger -> ILogger rejestrowany z JSON (zamiast imperatywnego wpisu z części 3)
// builder.RegisterType<Logger>().As<ILogger>();
builder.RegisterModule(new ConfigurationModule(config.GetSection("autofac")));

// --- Część 3 ---
builder.RegisterType<Worker>();

// --- Część 4 ---
builder.RegisterType<NumLogger>()
       .Named<ILogger>("numlog")
       .WithParameter("initialValue", 100)
       .SingleInstance();
builder.RegisterType<PropertyWorker>()
       .OnActivated(e => e.Instance.Logger = e.Context.ResolveNamed<ILogger>("numlog"));
builder.RegisterType<MethodWorker>()
       .OnActivated(e => e.Instance.SetLogger(e.Context.ResolveNamed<ILogger>("numlog")));

// --- Część 5 ---
builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

// --- Część 6 ---
builder.RegisterType<TransactionContext>()
       .As<ITransactionContext>()
       .InstancePerMatchingLifetimeScope("transaction");
builder.RegisterType<StepService>();

using var container = builder.Build();

// ===== Część 3 + 7 =====
Console.WriteLine("=== Część 3/7: Worker (ILogger z appsettings.json) ===");
container.Resolve<Worker>().Execute(1);

// ===== Część 4 =====
Console.WriteLine("\n=== Część 4: wstrzykiwanie przez właściwość i metodę ===");
container.Resolve<PropertyWorker>().Work();
container.Resolve<MethodWorker>().Work();
container.Resolve<PropertyWorker>().Work();

// ===== Część 5 =====
Console.WriteLine("\n=== Część 5: zasięgi ===");
Guid idScope1;
using (var scope1 = container.BeginLifetimeScope())
{
    var uowA = scope1.Resolve<IUnitOfWork>();
    var uowB = scope1.Resolve<IUnitOfWork>();
    Console.WriteLine($"scope1 #1: {uowA.Id}");
    Console.WriteLine($"scope1 #2: {uowB.Id}");
    Console.WriteLine($"Ta sama instancja w scope1: {ReferenceEquals(uowA, uowB)}");
    idScope1 = uowA.Id;
}
using (var scope2 = container.BeginLifetimeScope())
{
    var uowC = scope2.Resolve<IUnitOfWork>();
    Console.WriteLine($"scope2:    {uowC.Id}");
    Console.WriteLine($"Inna niż w scope1: {uowC.Id != idScope1}");
}

// ===== Część 6 =====
Console.WriteLine("\n=== Część 6: zasięgi otagowane ===");
try
{
    container.Resolve<ITransactionContext>();
    Console.WriteLine("BŁĄD: rozwiązano poza transakcją!");
}
catch (DependencyResolutionException ex)
{
    Console.WriteLine($"Oczekiwany wyjątek: {ex.GetType().Name}");
    Console.WriteLine(ex.Message);
}

Guid tx1;
using (var txScope = container.BeginLifetimeScope("transaction"))
{
    var s1 = txScope.Resolve<StepService>();
    var s2 = txScope.Resolve<StepService>();
    Console.WriteLine($"Transakcja 1, krok 1: {s1.GetCurrentTransaction()}");
    Console.WriteLine($"Transakcja 1, krok 2: {s2.GetCurrentTransaction()}");
    using (var inner = txScope.BeginLifetimeScope())
    {
        Console.WriteLine($"Transakcja 1, zasięg wewn.: {inner.Resolve<StepService>().GetCurrentTransaction()}");
    }
    tx1 = s1.GetCurrentTransaction();
}
using (var txScope2 = container.BeginLifetimeScope("transaction"))
{
    var s = txScope2.Resolve<StepService>();
    Console.WriteLine($"Transakcja 2:         {s.GetCurrentTransaction()}");
    Console.WriteLine($"Inna niż transakcja 1: {s.GetCurrentTransaction() != tx1}");
}