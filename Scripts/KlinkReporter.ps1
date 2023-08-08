#$exePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$exePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$workingDir = 'C:\Klink'
$projectFolder = 'Shc'
$configCount = '10'
$iterationCount = '10'
$procedure = 'reportAll'

$processOptions = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory ${workingDir} --project-folder ${projectFolder} --procedure ${procedure} --starting-config-index 0 --config-count ${configCount} --iteration-count ${iterationCount} --log-level 6"
}
Start-Process @processOptions




