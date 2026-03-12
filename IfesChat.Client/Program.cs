using System.Net.Sockets;
using System.Text;


string serverIP = "26.194.71.69";
int port = 5000;

try
{
    Console.WriteLine("Digite seu nome: ");
    string? nome = Console.ReadLine();
    
    using TcpClient client = new TcpClient();
    await client.ConnectAsync(serverIP, port);
    
    using NetworkStream stream = client.GetStream();
    
    byte[] sendNameBuffer = Encoding.UTF8.GetBytes(nome);
    await stream.WriteAsync(sendNameBuffer, 0, sendNameBuffer.Length);
    
    
    Console.WriteLine($"[CLIENTE] {nome} Conectado! Digite suas mensagens (ou 'sair' para encerrar):");
    
    _ = Task.Run(async () =>
    {
        byte[] receiveBuffer = new byte[1024];
        while (true)
        {
            int bytesRead = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
            if (bytesRead == 0) break;

            string resposta = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
            Console.WriteLine($"\n[SERVIDOR]: {resposta}");
            Console.Write("> "); 
        }
    });
    
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


