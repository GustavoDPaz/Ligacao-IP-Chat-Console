using System.Net.Sockets;
using System.Text;

string serverIP = "26.194.71.69";
int port = 5000;

try
{
    using TcpClient client = new TcpClient();
    await client.ConnectAsync(serverIP, port);
    Console.WriteLine("[CLIENTE] Conectado! Digite suas mensagens (ou 'sair' para encerrar):");

    using NetworkStream stream = client.GetStream();
    
    // Task para ficar ouvindo o servidor em segundo plano
    _ = Task.Run(async () =>
    {
        byte[] receiveBuffer = new byte[1024];
        while (true)
        {
            int bytesRead = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
            if (bytesRead == 0) break;

            string resposta = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
            Console.WriteLine($"\n[SERVIDOR]: {resposta}");
            Console.Write("> "); // Volta o prompt para o usuário
        }
    });

    // Loop principal para enviar mensagens
    while (true)
    {
        Console.Write("> ");
        string? mensagem = Console.ReadLine();

        if (string.IsNullOrEmpty(mensagem) || mensagem.ToLower() == "sair") 
            break;

        byte[] sendBuffer = Encoding.UTF8.GetBytes(mensagem);
        await stream.WriteAsync(sendBuffer, 0, sendBuffer.Length);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ERRO]: {ex.Message}");
}

Console.WriteLine("Saindo do chat...");