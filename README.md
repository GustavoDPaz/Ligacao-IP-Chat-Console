# Chat TCP em C#

## Introdução

Este projeto implementa um **sistema de chat em tempo real baseado em TCP** utilizando **C# e .NET**.
Ele é composto por duas aplicações principais:

* **Servidor de Chat**: responsável por aceitar conexões de múltiplos clientes e retransmitir mensagens.
* **Cliente de Chat**: conecta ao servidor e permite que usuários enviem e recebam mensagens.

A comunicação entre cliente e servidor é feita através de **sockets TCP**, garantindo uma conexão persistente para troca de mensagens em tempo real.


# Arquitetura

O sistema utiliza uma arquitetura **Cliente–Servidor**.

```
+-------------+        TCP        +-------------+
|  Cliente 1  |  <------------>   |             |
+-------------+                   |             |
                                  |             |
+-------------+        TCP        |   Servidor  |
|  Cliente 2  |  <------------>   |    de Chat  |
+-------------+                   |             |
                                  |             |
+-------------+        TCP        |             |
|  Cliente N  |  <------------>   |             |
+-------------+                   +-------------+
```

Fluxo básico:

1. O servidor inicia e escuta conexões em uma porta.
2. Clientes conectam ao servidor.
3. Cada cliente envia seu nome ao conectar.
4. Quando um cliente envia uma mensagem:

   * O servidor recebe
   * O servidor retransmite para todos os outros clientes conectados.

---

# Funcionalidades

* Conexão de múltiplos clientes simultaneamente
* Comunicação em tempo real
* Identificação de usuários por nome
* Broadcast de mensagens para todos os clientes
* Uso de programação assíncrona (`async/await`)
* Execução concorrente utilizando `Task`
* Controle de concorrência com `lock`

---

# Tecnologias Utilizadas

* **C#**
* **.NET**
* **Sockets TCP**
* **Programação Assíncrona**
* **Tasks e Multithreading**

Bibliotecas principais:

* `System.Net`
* `System.Net.Sockets`
* `System.Text`
* `System.Linq`

---

# Instalação

## 1. Clonar o repositório

```bash
git clone https://github.com/GustavoDPaz/Ligacao-IP-Chat-Console.git
cd Ligacao-IP-Chat-Console
```

## 2. Abrir no Visual Studio ou VS Code

Abra a pasta do projeto na IDE de sua preferência.

---

# Execução

## Iniciar o servidor

```bash
dotnet run
```

O servidor iniciará escutando conexões na porta:

```
5000
```

Exemplo de saída:

```
[SERVIDOR] Sala de Chat ativa na porta 5000...
```

---

## Iniciar um cliente

Execute o projeto do cliente em outro terminal:

```bash
dotnet run
```

O programa solicitará o nome do usuário:

```
Digite seu nome:
```

Após conectar:

```
[CLIENTE] João Conectado! Digite suas mensagens:
>
```

---

# Uso

1. Execute o **servidor**.
2. Execute **um ou mais clientes**.
3. Cada cliente digita um nome.
4. Mensagens enviadas serão exibidas para todos os usuários conectados.

Para sair do chat:

```
sair
```

---

# Fluxo de Funcionamento

### Conexão

1. Cliente conecta ao servidor via TCP.
2. Cliente envia seu nome.
3. Servidor adiciona o cliente à lista de usuários conectados.

### Envio de mensagens

1. Cliente envia mensagem ao servidor.
2. Servidor recebe mensagem.
3. Servidor envia mensagem para todos os outros clientes.

### Desconexão

1. Cliente encerra conexão.
2. Servidor remove o usuário da lista.
3. Mensagem de desconexão é registrada no console.

---

# Exemplo de Uso

Servidor:

```
[SERVIDOR] Sala de Chat ativa na porta 5000...
[CONECTADO] João entrou.
[CONECTADO] Maria entrou.
[João]: Olá pessoal!
[Maria]: Oi João!
```

Cliente:

```
Digite seu nome:
João

[CLIENTE] João Conectado!
> Olá pessoal!
```

---

# Dependências

O projeto utiliza apenas bibliotecas padrão do .NET:

* `System.Net`
* `System.Net.Sockets`
* `System.Text`
* `System.Threading.Tasks`
* `System.Linq`

Não há dependências externas.

---

# Configuração

### Alterar IP do servidor

No cliente, modifique:

```csharp
string serverIP = "26.194.71.69";
```

### Alterar porta

No servidor:

```csharp
int port = 5000;
```

E no cliente:

```csharp
int port = 5000;
```

---

# Estrutura do Projeto

```
chat-tcp-csharp
│
├── servidor
│   └── Program.cs
│
├── cliente
│   └── Program.cs
│
└── README.md
```

---

# Tratamento de Erros

O projeto inclui tratamento básico de exceções para:

* Falha de conexão
* Desconexão inesperada
* Problemas de rede

Erros são exibidos no console:

```
[ERRO]: mensagem do erro
```

---

# Melhorias Futuras

Algumas melhorias possíveis:

* Interface gráfica (GUI)
* Suporte a salas de chat
* Histórico de mensagens
* Autenticação de usuários
* Criptografia das mensagens
* Comandos administrativos
* Sistema de reconexão automática

---

# Troubleshooting

### Não conecta ao servidor

Verifique:

* Se o servidor está rodando
* Se o IP do servidor está correto
* Se a porta está liberada no firewall

### Mensagens não aparecem

Pode ser causado por:

* Perda de conexão
* Cliente encerrado
* Problemas de rede

