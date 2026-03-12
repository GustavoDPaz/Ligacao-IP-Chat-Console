using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

List<Usuario> clientes = new List<Usuario>();
object travaLista = new object();

IPAddress ipAddress = IPAddress.Any;
int port = 5000;



TcpListener server = new TcpListener(ipAddress, port);
server.Start();

Console.WriteLine($"[SERVIDOR] Sala de Chat ativa na porta {port}...");


while (true)
{
    TcpClient client = await server.AcceptTcpClientAsync();
    

    _ = Task.Run(() => HandleClient(client));
}


async Task HandleClient(TcpClient client)
{
    using NetworkStream stream = client.GetStream();
    byte[] buffer = new byte[1024];
    
    int nome_bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
    string nome = Encoding.UTF8.GetString(buffer, 0, nome_bytes);
    
    Usuario NovoUsuario = new Usuario(client, nome);
    lock (travaLista) { clientes.Add(NovoUsuario); }
    
    Console.WriteLine($"[CONECTADO] {nome} entrou.");
    
    try
    {
        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) break;

            string mensagem = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string mensagemFormatada = $"[{nome}]: {mensagem}";
            
            Console.WriteLine(mensagemFormatada);
            await EnviarParaTodos(mensagemFormatada, client);
        }
    }
    catch (Exception) { }
    finally
    {
        lock (travaLista) { clientes.Remove(NovoUsuario); }
        Console.WriteLine($"[SAIU] {nome} desconectou.");
        client.Close();
    }
}

async Task EnviarParaTodos(string mensagem, TcpClient remetente)
{
    byte[] buffer = Encoding.UTF8.GetBytes(mensagem);
    
    List<Task> tarefasDeEnvio = new List<Task>();
    
    lock (travaLista)
    {
        var envios = clientes
            .Where(c => c.Conexao != remetente)
            .Select(c => c.Conexao.GetStream().WriteAsync(buffer, 0, buffer.Length));
        
        tarefasDeEnvio.AddRange(envios);
    }
    await Task.WhenAll(tarefasDeEnvio);
}

public class Usuario(TcpClient client, string nome)
{
    public TcpClient Conexao { get; set; } = client;
    public string Nome { get; set; } = nome;
}
