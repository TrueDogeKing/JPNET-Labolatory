PARAM (
    [string] $SolutionFilesPath = "."
)
$folders_To_delete = @('*packages*','*.vs*','*bin*','*x64*','*.tlog*','*csx*','*ecf*','*obj*','*rcf*')

function Remove-Tree-Directly($Path)
{ 
    Remove-Item $Path -force -Recurse -ErrorAction silentlycontinue

    if (Test-Path "$Path\" -ErrorAction silentlycontinue)
    {
        $folders = Get-ChildItem -Path $Path –Directory -Force
        ForEach ($folder in $folders)
        {
            Remove-Tree $folder.FullName
        }

        $files = Get-ChildItem -Path $Path -File -Force

        ForEach ($file in $files)
        {
            Remove-Item $file.FullName -force
        }

        if (Test-Path "$Path\" -ErrorAction silentlycontinue)
        {
            Remove-Item $Path -force
        }
    }
}

function Remove-Tree($Path, $Matches)
{
    if (Test-Path "$Path\" -ErrorAction silentlycontinue)
    {
        $folders = Get-ChildItem -Path $Path –Directory -Force
        ForEach ($folder in $folders)
        {
            ForEach ($Match in $Matches)
            {
                if ($folder -like $Match)
                {
                    Remove-Tree-Directly $folder.FullName
                    break
                }
            }
            Remove-Tree $folder.FullName $Matches
        }
    }
}

$folders = Get-ChildItem -Path $SolutionFilesPath –Directory -Force
ForEach ($folder in $folders)
{
    Remove-Tree $folder.FullName $folders_To_delete
}

$extensions = @('*.pdb','*.obj','*.idb','*.pdb','*.log','*.tlog','*.ilk','*.recipe','*.db','*.suo','*.lastbuildstate','_IsIncrementalBuild','unsuccessfulbuild')
get-childitem $SolutionFilesPath -recurse -force -include $extensions | remove-item -force
