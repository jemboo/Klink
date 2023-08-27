#$exePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$exePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$workingDir = "Klink"
$projectFolder = "Shc"

$processOptions = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory Klink --project-folder Shc --procedure doRun --starting-config-index 4 --config-count 2 --log-level 6"
}
Start-Process @processOptions

$processOptions2 = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory Klink --project-folder Shc --procedure doRun --starting-config-index 6 --config-count 3 --log-level 6"
}
Start-Process @processOptions2



