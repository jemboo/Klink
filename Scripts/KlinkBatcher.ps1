$processOptions = @{
    FilePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory Yowza --project-folder Gort --run-folder Shc --control-file ShcBatches --starting-config-index 1 --config-count 3 --log-level 6"
}
Start-Process @processOptions

$processOptions2 = @{
    FilePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory Yowza --project-folder Gort --run-folder Shc --control-file ShcBatches --starting-config-index 4 --config-count 3 --log-level 6"
}
Start-Process @processOptions2



