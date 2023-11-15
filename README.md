# Telegram Freelancer Bot

Este é um bot Telegram simples para gerenciar freelancers, permitindo a inclusão de novos freelancers e a listagem de freelancers existentes.

## Requisitos

- .NET Core 5.0 ou superior
- MySQL Server
- Token de Bot Telegram

## Configuração

1. Clone este repositório:

    ```bash
    git clone https://github.com/seu-usuario/seu-repositorio.git
    cd seu-repositorio
    ```

2. Abra o arquivo `app.config` e configure o token do bot e a string de conexão do banco de dados:

    ```xml
    <appSettings>
        <add key="BotToken" value="SEU_TOKEN_AQUI" />
        <add key="TelegramBotConnection" value="Server=SEU_SERVER;Database=telegram_bot;Uid=SEU_USUARIO;Pwd=SUA_SENHA;" />
    </appSettings>
    ```

    Substitua `SEU_TOKEN_AQUI`, `SEU_SERVER`, `SEU_USUARIO` e `SUA_SENHA` pelos valores reais do seu bot e do seu banco de dados MySQL.

3. Execute o script SQL fornecido (`databasestart.sql`) no seu MySQL para criar o banco de dados e a tabela necessários.

4. Build e execute o projeto:

    ```bash
    dotnet build
    dotnet run
    ```

## Comandos Disponíveis

- `/start` - Inicia o bot
- `/includeFreelancer` \<Nome> \<Experiência> \<Descrição> - Adiciona um novo freelancer
- `/listFreelancers` - Lista todos os freelancers

## Contribuição

Sinta-se à vontade para contribuir, reportar problemas ou fazer melhorias neste projeto. O bot foi desenvolvido em C# usando a biblioteca [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) e o MySQL como banco de dados.