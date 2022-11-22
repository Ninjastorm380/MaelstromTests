Imports System.Threading
Imports Maelstrom

Friend Class TestClient : Inherits ClientBase
    Private Payload as Byte() =
                {1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, 
                 1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1}
    
    Private ReadOnly WaitLock As Object = New Object()
    
    Dim Speed As Double = 1
    Friend Sub SetSpeed(Rate as Double)
        Speed = Rate
    End Sub
    
    Dim Instances As Double = 1
    Friend Sub SetInstances(Amount as UInt32)
        Instances = Amount
    End Sub
    
    Private Sub CreateAsyncInstance(Socket as Socket, SubSocket as Int32)
        Dim AsyncThread as new Thread(
            Sub()
                Dim Governor as new Governor(Speed)
                Dim Buffer(Payload.Length - 1) as Byte
                Dim Compared as Boolean
                Socket.Add(SubSocket)
                Socket.Compress(Subsocket) = True
                Socket.Encrypt(Subsocket) = True
                
                Console.WriteLine("DEBUG - CLIENT - ASYNC INSTANCE " + SubSocket.ToString() + ": awaiting unlock...") 
                SyncLock WaitLock : End SyncLock
                Console.WriteLine("DEBUG - CLIENT - ASYNC INSTANCE " + SubSocket.ToString() + ": unlocked!") 
                Thread.Sleep(1000)
                Console.WriteLine("DEBUG - CLIENT - ASYNC INSTANCE " + SubSocket.ToString() + ": writing...") 
                Socket.Write(SubSocket, Payload)
                Console.WriteLine("DEBUG - CLIENT - ASYNC INSTANCE " + SubSocket.ToString() + ": written!") 
                
                Do While Socket.Connected = True
                    If Socket.Pending(SubSocket) = True Then
                        Socket.Read(SubSocket, Buffer)
                        Compared = BinaryCompare(Buffer, Payload, 0, Buffer.Length)

                        If Compared = False Then Socket.Disconnect()
                        Socket.Write(SubSocket, Payload)
                        'Console.WriteLine("DEBUG - CLIENT - ASYNC INSTANCE " + SubSocket.ToString() + ": integerity is " + Compared.ToString().ToLower() + ", execution time: " + Governor.Delta.ToString())
                    End If
                    If Governor.Delta > 1.1 Then
                        Console.WriteLine("DEBUG - CLIENT - ASYNC INSTANCE " + SubSocket.ToString() + ": execution time: " + Governor.Delta.ToString() + " - overloaded!")
                    Else
                        Console.WriteLine("DEBUG - CLIENT - ASYNC INSTANCE " + SubSocket.ToString() + ": execution time: " + Governor.Delta.ToString() + " - ok!")
                    End If
                    Governor.Limit()
                Loop
                Socket.Remove(SubSocket)
            End Sub)
        AsyncThread.Start()
    End Sub

    Public Overrides Sub Main(Socket As Socket)
        Console.WriteLine("DEBUG - CLIENT: connected to server")
        Console.WriteLine("DEBUG - CLIENT: creating async instances....") 
        SyncLock WaitLock
            For x = 0 to Instances - 1
                CreateAsyncInstance(Socket,x)
                Console.WriteLine("DEBUG - CLIENT: async instance " + x.ToString() + " created.") 
            Next
        End SyncLock
        Console.WriteLine("DEBUG - CLIENT: all async instances created.") 
    End Sub
End Class