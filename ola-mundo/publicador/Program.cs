using RabbitMQ.Client;
using System;
using System.Text;

namespace publicador
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Exemplo 1 - Publicador";
            Menu();
        }

        static void Menu()
        {
            Console.WriteLine("\nTecle ENTER para digitar uma mensagem");
            Console.WriteLine("Tecle ESC para encerrar o programa");
            OpcaoSelecionada();
        }

        static void OpcaoSelecionada()
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            cki = Console.ReadKey(true);

            if (cki.Key == ConsoleKey.Enter)
            {
                string mensagem = string.Empty;
                while (string.IsNullOrEmpty(mensagem))
                {
                    Console.Write("\nDigite a mensagem:");
                    mensagem = Console.ReadLine();
                    if (string.IsNullOrEmpty(mensagem))
                    {
                        Console.WriteLine("\nMensagem inválida!");
                    }
                }
                
                EnviarMensagem(mensagem);
            }
            else if (cki.Key == ConsoleKey.Escape)
            {
                return;
            }
            else
            {
                Console.WriteLine("Tecla inválida");
            }

            Menu();
        }

        static void EnviarMensagem(string mensagem)
        {
            const string fila = "TesteBlog";

            try
            {
                /* Se a aplicação estiver no mesmo servidor do Rabbit, o hostname pode ser informado como 
                "localhost" e não será necessário usuário e senha. Por padrão será o usuário "guest".
                A menos que deseje acessar usando outro usuário. */
                var factory = new ConnectionFactory() 
                { 
                    HostName = "emu.rmq.cloudamqp.com",
                    UserName = "kcdrbgsw",
                    Password = "GQNUnn1SZrONcExUvNQRYWgcVXGBYj3l",
                    VirtualHost = "kcdrbgsw"
                    // Ou
                    //Uri = new Uri("amqp://kcdrbgsw:GQNUnn1SZrONcExUvNQRYWgcVXGBYj3l@emu.rmq.cloudamqp.com/kcdrbgsw")
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: fila,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );

                        var body = Encoding.UTF8.GetBytes(mensagem);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: fila,
                            basicProperties: null,
                            body: body
                        );

                        Console.WriteLine("\nMensagem enviada com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao enviar mensagem! \nExcessão: {ex}");
            }
        }
    }
}