using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace consumidor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Exemplo 1 - Consumidor";
            Menu();
        }

        static void Menu()
        {
            Console.WriteLine("\nTecle ENTER para buscar mensagens");
            Console.WriteLine("Tecle ESC para encerrar o programa");
            OpcaoSelecionada();
        }

        static void OpcaoSelecionada()
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            cki = Console.ReadKey(true);

            if (cki.Key == ConsoleKey.Enter)
            {
                LerMensagem();
            }
            else if (cki.Key == ConsoleKey.Escape)
            {
                return;
            }
            else
            {
                Console.WriteLine("Tecla inválida!");
            }

            Menu();
        }

        static void LerMensagem()
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

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            if (body == null)
                            {
                                Console.WriteLine("\nNão há mensagens na fila!");
                            }
                            else
                            {
                                var message = Encoding.UTF8.GetString(body);
                                Console.WriteLine("\nMensagens:\n");
                                Console.WriteLine(message);
                            }
                        };

                        channel.BasicConsume(
                            queue: fila,
                            autoAck: true,
                            consumer: consumer
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFalha ao ler mensagens! \nExcessão: {ex}");
            }
        }
    }
}