Imports System.Threading

Module ModuleAck
    Sub Main(arg() As String)
        Const MB As ULong = 1024 * 1024
        Dim v1 As ULong
        Dim v2 As ULong
        Dim m As Integer = CInt(16 * MB)
        Dim sw As New Stopwatch()

        Select Case arg.Length
            Case < 2, > 3
                ShowUsage("Invalid Argument Count")
            Case Else
                If Not ULong.TryParse(arg(0), v1) Then ShowUsage($"Unable to parse 'm' value ({arg(0)})")

                If Not ULong.TryParse(arg(1), v2) Then ShowUsage($"Unable to parse 'n' value ({arg(1)})")

                If arg.Length = 3 Then
                    If arg(2).StartsWith("/s:") Then
                        Dim tokens() As String = arg(2).Split(":"c)
                        If Not Integer.TryParse(tokens(1), m) Then ShowUsage($"Unable to parse stack size ({tokens(1)})")
                        If m <= 0 Then
                            ShowUsage($"Invalid stack size ({m})")
                        End If
                        m = CInt(m * MB)
                    Else
                        ShowUsage($"Invalid third argument: ({arg(2)})")
                    End If
                End If
        End Select

        sw.Start()
        Console.WriteLine($"Ackermann({v1}, {v2}) = {Ack2(v1, v2, m):N0}")
        sw.Stop()
        Console.WriteLine($"{sw.Elapsed.Hours:00}:{sw.Elapsed.Minutes:00}:{sw.Elapsed.Seconds:00}:{sw.Elapsed.Milliseconds:000}")
        Console.WriteLine()

#If DEBUG Then
        Console.ReadKey()
#End If
    End Sub

    ' http://content.atalasoft.com/h/i/58213648-increasing-the-size-of-your-stack-net-memory-management-part-3
    Private Function Ack2(m As ULong, n As ULong, stackSize As Integer) As ULong
        Dim result As ULong
        Dim t As New Thread(Sub()
                                result = Ack(m, n)
                            End Sub, stackSize) With {.Priority = ThreadPriority.Highest}
        t.Start()
        t.Join()
        Return result
    End Function

    Private Function Ack(m As ULong, n As ULong) As ULong
        If m = 0 Then
            Return n + 1UL
        ElseIf n = 0 Then
            Return Ack(m - 1UL, 1UL)
        Else
            Return Ack(m - 1UL, Ack(m, n - 1UL))
        End If
    End Function

    Private Sub ShowUsage(Optional errMsg As String = "")
        Dim Wlwc = Sub(text As String, fc As ConsoleColor)
                       Dim dfc As ConsoleColor = Console.ForegroundColor
                       Console.ForegroundColor = fc
                       Console.WriteLine(text)
                       Console.ForegroundColor = dfc
                   End Sub

        Wlwc($"{My.Application.Info.AssemblyName} {My.Application.Info.Version}", ConsoleColor.White)
        Console.WriteLine()
        If errMsg <> "" Then
            Wlwc(errMsg, ConsoleColor.Red)
            Console.WriteLine()
        End If
        Console.WriteLine("Usage:")
        Wlwc($"{My.Application.Info.AssemblyName} m n [/s:StackSize]", ConsoleColor.White)
        Console.WriteLine()
        Console.WriteLine("m:  Positive Number")
        Console.WriteLine("n:  Positive Number")
        Console.WriteLine("/s: Optional positive number that defines the Stack Size in MegaBytes.")
        Console.WriteLine("    If not used, the default Stack Size will be 16MB")
        Console.WriteLine()

        Environment.Exit(1)
    End Sub
End Module