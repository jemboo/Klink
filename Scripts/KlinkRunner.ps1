#$exePath = "C:\Users\jembo\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$exePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
$workingDir = 'C:\Klink'

$processOptions = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
}
Start-Process @processOptions


$processOptions2 = @{
    FilePath = "C:\source\Klink\Klink.Runner\bin\Release\net6.0\Klink.Runner.exe"
}
Start-Process @processOptions2



