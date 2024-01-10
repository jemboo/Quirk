#--working-directory C:\Quirk --project-name Shc_064b --cfgplex-name Shc_064_cfgPlex --program-mode RunScript --first-script-index 0 --script-count 400 --report-file-name bins --use-parallel true --log-level 1

#$exePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$exePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$workingDir = 'C:\Klink'

$processOptions = @{
    FilePath = "C:\source\Quirk\Quirk.Runner\bin\Debug\net8.0\Quirk.Runner.exe"    
    ArgumentList = " --working-directory C:\Quirk --project-name Shc_064b --cfgplex-name Shc_064_cfgPlex --program-mode RunScript --first-script-index 0 --script-count 400 --report-file-name bins --use-parallel true --log-level 1"
}
Start-Process @processOptions


$processOptions2 = @{
    FilePath = "C:\source\Quirk\Quirk.Runner\bin\Debug\net8.0\Quirk.Runner.exe"    
    ArgumentList = " --working-directory C:\Quirk --project-name Shc_064b --cfgplex-name Shc_064_cfgPlex --program-mode RunScript --first-script-index 0 --script-count 400 --report-file-name bins --use-parallel true --log-level 1"
}
Start-Process @processOptions2



$processOptions3 = @{
    FilePath = "C:\source\Quirk\Quirk.Runner\bin\Debug\net8.0\Quirk.Runner.exe"    
    ArgumentList = " --working-directory C:\Quirk --project-name Shc_064b --cfgplex-name Shc_064_cfgPlex --program-mode RunScript --first-script-index 0 --script-count 400 --report-file-name bins --use-parallel true --log-level 1"
}
Start-Process @processOptions3
