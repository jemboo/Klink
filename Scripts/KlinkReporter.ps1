#$exePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$exePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$workingDir = 'C:\Shc'
$projectFolder = 'StageW'
$reportFileName = 'deucey'
$configCount = '18'
$firstConfigIndex = '0'
$iterationCount = '10'
$procedure = 'reportAll'

$processOptions = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
    ArgumentList = " --working-directory ${workingDir} --project-folder ${projectFolder}  --report-file-name ${reportFileName} --procedure ${procedure} --starting-config-index ${firstConfigIndex} --config-count ${configCount} --iteration-count ${iterationCount} --log-level 6"
}
Start-Process @processOptions




