using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics; // Movido para o topo

// --- CONFIGURAÇÃO ---
List<TcpClient> clientes = new List<TcpClient>();
object travaLista = new object();

IPAddress ipAddress = IPAddress.Any;
int port = 5000;

// 1. CHAMADA DO MÉTODO: Você precisa chamar a função para ela ser executada!
LiberarFirewall();

TcpListener server = new TcpListener(ipAddress, port);
server.Start();

Console.WriteLine($"[SERVIDOR] Sala de Chat ativa na porta {port}...");
Console.WriteLine("[DICA] Se o Windows pediu permissão (UAC), a porta foi aberta.");

// --- LOOP PRINCIPAL ---
while (true)
{
    TcpClient client = await server.AcceptTcpClientAsync();
    
    lock (travaLista)
    {
        clientes.Add(client);
    }

    _ = Task.Run(() => HandleClient(client));
}

// ==========================================
// MÉTODOS (FUNÇÕES)
// ==========================================

static void LiberarFirewall()
{
    try
    {
        Console.WriteLine("[FIREWALL] Solicitando abertura da porta 5000...");
        
        string comando = "New-NetFirewallRule -DisplayName 'Chat IFES' -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow -ErrorAction SilentlyContinue";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{comando}\"",
            Verb = "runas", // Força o pedido de Administrador
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);
    }
    catch (Exception)
    {
        Console.WriteLine("[AVISO] Permissão negada. O Firewall pode bloquear outros PCs.");
    }
}

async Task HandleClient(TcpClient client)
{
    string endPoint = client.Client.RemoteEndPoint?.ToString() ?? "Anônimo";
    Console.WriteLine($"[CONECTADO] {endPoint} entrou.");

    using NetworkStream stream = client.GetStream();
    byte[] buffer = new byte[1024];

    try
    {
        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) break;

            string mensagem = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string mensagemFormatada = $"[{endPoint}]: {mensagem}";
            
            Console.WriteLine(mensagemFormatada);
            EnviarParaTodos(mensagemFormatada, client);
        }
    }
    catch (Exception) { }
    finally
    {
        lock (travaLista) { clientes.Remove(client); }
        Console.WriteLine($"[SAIU] {endPoint} desconectou.");
        client.Close();
    }
}

void EnviarParaTodos(string mensagem, TcpClient remetente)
{
    byte[] buffer = Encoding.UTF8.GetBytes(mensagem);
    lock (travaLista)
    {
        foreach (var c in clientes)
        {
            if (c != remetente)
            {
                try
                {
                    NetworkStream stream = c.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch { }
            }
        }
    }
}