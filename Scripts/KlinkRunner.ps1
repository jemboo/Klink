#$exePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$exePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$workingDir = 'C:\Shc'
$projectFolder = 'StageW'
$reportFileName = 'deucey'
$procedure = 'doRun'
$configCount = '12'
$iterationCount = '4000'
#$procedure = 'doRun'

$processOptions = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory ${workingDir} --project-folder ${projectFolder} --procedure ${procedure} --starting-config-index 0 --config-count ${configCount} --iteration-count ${iterationCount} --log-level 6"
}
Start-Process @processOptions

$processOptions2 = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory ${workingDir} --project-folder ${projectFolder} --procedure ${procedure} --starting-config-index ${configCount} --config-count ${configCount} --iteration-count ${iterationCount} --log-level 6"
}
Start-Process @processOptions2



