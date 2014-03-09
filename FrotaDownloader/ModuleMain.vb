Imports System
Imports System.Net
Imports System.IO
Imports Microsoft.Win32
Imports System.ComponentModel

Module ModuleMain
    Dim Dota2Path As String
    Dim Dota2AddonPath As String

    Sub Main()
        IsDotaAvailble()
        Console.WriteLine("Dota 2 addonpath: " + Dota2AddonPath)
        Try
            System.IO.Directory.Delete(Dota2AddonPath + "\Frota", True)
        Catch
        End Try
        Try
            System.IO.Directory.Delete(Dota2AddonPath + "\Frota-master", True)
        Catch
        End Try
        Try
            My.Computer.FileSystem.RenameDirectory(Dota2AddonPath + "\Frota-master", "Frota")
        Catch
        End Try
        download("https://github.com/ash47/Frota/archive/master.zip", Dota2AddonPath + "\frota.zip")
        Console.Read()
    End Sub

    Public Sub IsDotaAvailble()
        ' Check if it's 64 bit system
        Dim Is64Bit As Boolean = Environment.Is64BitOperatingSystem

        ' Get registery key
        Dim dota2Key = Registry.LocalMachine.OpenSubKey("SOFTWARE\" + If(Is64Bit, "Wow6432Node\", "") + "Microsoft\Windows\CurrentVersion\Uninstall\Steam App 570")
        If Not String.IsNullOrEmpty(dota2Key.ToString) Then
            ' Get installation directory of Dota2
            Dim dotaKeyValue = dota2Key.GetValue("InstallLocation")
            If Not String.IsNullOrEmpty(dotaKeyValue.ToString) Then
                Dota2Path = dotaKeyValue.ToString()
                Dota2AddonPath = Dota2Path + "\dota\addons"
                Return
            End If
        End If
    End Sub

    Public Sub download(address As String, myFile As String)
        Dim client As WebClient = New WebClient()

        AddHandler client.DownloadFileCompleted, AddressOf DownloadFileCompletedCallback
        AddHandler client.DownloadProgressChanged, AddressOf DownloadProgressCallback

        Dim uri As Uri = New Uri(address)
        Console.WriteLine("download started ...")
        client.DownloadFileAsync(uri, myFile)
    End Sub

    Dim percent As Integer = -1
    Private Sub DownloadProgressCallback(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        If percent < e.ProgressPercentage Then
            Console.WriteLine("Download status: " + e.BytesReceived.ToString + "bytes of " + e.TotalBytesToReceive.ToString + "bytes (" + e.ProgressPercentage.ToString + "%)")
            percent = e.ProgressPercentage
        End If
    End Sub

    Private Sub DownloadFileCompletedCallback(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)
        Console.WriteLine("download finished")
        Dim file As String = Dota2AddonPath + "\frota.zip"
        Dim cu As New ClassUnzip(file, Path.Combine(Path.GetDirectoryName(file)))
        AddHandler cu.UnzipFinishd, AddressOf Unziped
        Console.WriteLine("start unziping ...")
        cu.UnzipNow()
    End Sub

    Private Sub Unziped()
        Console.WriteLine("rename frota folder")
        My.Computer.FileSystem.RenameDirectory(Dota2AddonPath + "\Frota-master", "Frota")
        Console.WriteLine("delete frota.zip")
        File.Delete(Dota2AddonPath + "\frota.zip")
        Console.WriteLine("frota installation completed")
        Console.WriteLine("Press 'return' to exit ...")
    End Sub
End Module
