#include "pch.h"
#include "LabCli.h"

namespace LabCli {

    // Brak x:Name / Name - wszystko przez Binding (czysty MVVM).
    static const char* MainWindowXaml = R"XAML(
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        Title="Lab 5 - C++/CLI + Roslyn + Reflection" Width="1000" Height="650">
  <Grid Margin="8">
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="2*"/>
      <ColumnDefinition Width="10"/>
      <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>

    <!-- LEWA STRONA: kod, kompilacja, bledy -->
    <Grid Grid.Column="0">
      <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="3*"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
      </Grid.RowDefinitions>
      <TextBlock Text="VB.NET code" FontWeight="Bold"/>
      <TextBox Grid.Row="1" Text="{Binding Code, UpdateSourceTrigger=PropertyChanged}"
               AcceptsReturn="True" AcceptsTab="True" FontFamily="Consolas" FontSize="13"
               HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto"/>
      <StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,6">
        <Button Content="Compile" Width="100" Command="{Binding CompileCommand}"/>
        <TextBlock Text="Kind of output:" VerticalAlignment="Center" Margin="12,0,4,0"/>
        <ComboBox Width="220" ItemsSource="{Binding OutputKinds}"
                  SelectedItem="{Binding SelectedOutputKind}"/>
      </StackPanel>
      <TextBlock Grid.Row="3" Text="Errors" FontWeight="Bold"/>
      <ListBox Grid.Row="4" ItemsSource="{Binding Errors}" Foreground="DarkRed"/>
    </Grid>

    <!-- PRAWA STRONA: metody i zmienne -->
    <Grid Grid.Column="2">
      <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
      </Grid.RowDefinitions>

      <TextBlock Grid.Row="0" Text="Methods" FontWeight="Bold"/>
      <ListBox Grid.Row="1" ItemsSource="{Binding Methods}"
               SelectedIndex="{Binding SelectedMethodIndex}"/>
      <DockPanel Grid.Row="2" Margin="0,6,0,0">
        <TextBlock DockPanel.Dock="Left" Text="Parameter:" Width="70" VerticalAlignment="Center"/>
        <Button DockPanel.Dock="Right" Content="INVOKE" Width="70" Margin="6,0,0,0"
                Command="{Binding InvokeCommand}"/>
        <TextBox Text="{Binding Parameter, UpdateSourceTrigger=PropertyChanged}"/>
      </DockPanel>
      <DockPanel Grid.Row="3" Margin="0,4,0,0">
        <TextBlock DockPanel.Dock="Left" Text="Result:" Width="70" VerticalAlignment="Center"/>
        <TextBox Text="{Binding Result}" IsReadOnly="True" Background="#F0F0F0"/>
      </DockPanel>

      <TextBlock Grid.Row="4" Text="Variables" FontWeight="Bold" Margin="0,12,0,0"/>
      <ListBox Grid.Row="5" ItemsSource="{Binding Variables}"
               SelectedIndex="{Binding SelectedVariableIndex}"/>
      <DockPanel Grid.Row="6" Margin="0,6,0,0">
        <TextBlock DockPanel.Dock="Left" Text="Type:" Width="70" VerticalAlignment="Center"/>
        <TextBox Text="{Binding VariableType}" IsReadOnly="True" Background="#F0F0F0"/>
      </DockPanel>
      <DockPanel Grid.Row="7" Margin="0,4,0,0">
        <TextBlock DockPanel.Dock="Left" Text="Value:" Width="70" VerticalAlignment="Center"/>
        <Button DockPanel.Dock="Right" Content="SET" Width="70" Margin="6,0,0,0"
                Command="{Binding SetCommand}"/>
        <TextBox Text="{Binding VariableValue, UpdateSourceTrigger=PropertyChanged}"/>
      </DockPanel>
    </Grid>
  </Grid>
</Window>)XAML";

    Window^ WindowFactory::Create() {
        String^ xaml = gcnew String(MainWindowXaml);
        auto reader = System::Xml::XmlReader::Create(gcnew StringReader(xaml));
        Window^ window = safe_cast<Window^>(XamlReader::Load(reader));   // jeden argument!
        window->DataContext = gcnew DataModel();
        return window;
    }
}
