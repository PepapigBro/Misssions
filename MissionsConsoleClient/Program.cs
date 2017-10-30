using MissionsConsoleClient.MissionsService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MissionsConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {

            string serviceUri = "";
            //Пользовательский ввод номера localhost и попытка соединения с ним
            ConnectToServer(ref serviceUri);
            var container = new Default.Container(new Uri(serviceUri));


            //Строковая комманда, введенная пользователем
            string command = "";
            //Главный цикл
            while (command != "e")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine();
                Console.WriteLine("Доступные комманды");
                Console.WriteLine(new string('-', 20));
                Console.WriteLine("1. getlist: получить весь список задач");
                Console.WriteLine("2. create: добавить новую задачу");
                Console.WriteLine("3. edit: редактировать задачу");
                Console.WriteLine("4. changestatus: изменить статус задачи ");
                Console.WriteLine("5. -exit: выход");
                Console.WriteLine(new string('-', 20));

                Console.Write("Ввести комманду: ");
                Console.ForegroundColor = ConsoleColor.White;
                command = Console.ReadLine();


                bool wantExit = false;
                switch (command)
                {
                    case "getlist":
                        // Получение и вывод в консоль всего списка задач
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("Waiting server response...");
                        ListAllMissions(container);
                        break;

                    case "create":

                        //Создание задачи с помощью пользовательского ввода
                        Mission newMission = MissionByUserInput();
                        if (newMission == null)
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Задача не создана");
                        }
                        else
                        {
                            //Отправка запроса на добавление новой задачи в db
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("Отправка на сервер..");
                            AddMission(container, newMission);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Задача создана");
                        }
                        break;

                    case "edit":
                        wantExit = EditMission(container);
                        if (wantExit)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Задача не изменена");
                            Console.WriteLine();

                        }
                        break;

                    case "changestatus":
                        wantExit = ChangeStatusMission(container);
                        if (wantExit)
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Статус не изменен");
                            Console.WriteLine();

                        }
                        break;

                }

            }

        }
        
        //Создание эксземпляра задачи с помощью пользовательского ввода
        private static Mission MissionByUserInput()
        {
            bool wantExist = false;
            Mission newMission = new Mission();
            Console.WriteLine();
            Console.WriteLine("Creation of Task");
            Console.WriteLine(new string('-', 20));

            //Ввод и валидация имени новой задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Task name (max 30 characters): ");
                newMission.Name = Console.ReadLine();
                if (newMission.Name == "-exit") { wantExist = true; break; }

                if (newMission.Name.Length <= 30 && newMission.Name.Length > 0) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    break; }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exceeded the maximum length 30 or Emtpy");
                    Console.WriteLine();
                }
            }

            //Ввод и валидация описания новой задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Task description (max 300 characters): ");
                newMission.Description = Console.ReadLine();
                if (newMission.Description == "-exit") { wantExist = true; break; }
                if (newMission.Description.Length <= 300) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    break; }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exceeded the maximum length 300");
                    Console.WriteLine();
                }
            }

            //Ввод и валидация дедлайна новой задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Deadline (DD-MM-YYYY hh:mm). Blank if not needed: ");
                string date = Console.ReadLine();
                if (date == "-exit") { wantExist = true; break; }
                if (!String.IsNullOrEmpty(date))
                {
                    try
                    {
                        newMission.Deadline = DateTime.ParseExact(date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("ok");
                        break;
                    }
                    catch
                    {
                        
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Incorrect date time format");
                        Console.WriteLine();
                    }
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    break;
                }
            }
            
            return newMission;
        }

        //Вывод одной задачи в консоль
        static void DisplayMission(Mission mission)
        {
            Console.WriteLine($"Id: {mission.Id}");

            Console.WriteLine($"Name: {mission.Name}");
            string deadline = mission.Deadline == null ? "отсутствует" : mission.Deadline.ToString();
            Console.WriteLine($"Deadline: {deadline}");

            string isDeferFromInitial = mission.IsDeferFromInitial == true ? ", Задача изменялась" : "";
            Console.WriteLine($"Status: {mission.TaskState}{isDeferFromInitial}");

            string dateOfCompletion = mission.DateOfCompletion == null ? "" : mission.DateOfCompletion.ToString();
            Console.WriteLine($"Дата выполнения: {dateOfCompletion}");

            Console.WriteLine($"Подробное описание: {mission.Description}");

            Console.WriteLine(new string('-', 20));
        }

        //Отправка запроса на сервер на получение всего списка задач
        //Вывод задач в консоль
        static void ListAllMissions(Default.Container container)
        {           
            var missionsList = container.Missions.ToList();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Список всех задач");
            Console.WriteLine(new string('-', 20));
            foreach (var mission in missionsList)
            {
                DisplayMission(mission);
            }
        }

        //Отправка запроса на сервер на создание новой задачи
        static void AddMission(Default.Container container, Mission mission)
        {
            container.AddToMissions(mission);
           
            var serviceResponse = container.SaveChanges();
            foreach (var operationResponse in serviceResponse)
            {
                Console.WriteLine("Response: {0}", operationResponse.StatusCode);
            }
        }

        //Отправка запроса на сервер на редактирование задачи
        private static bool EditMission(Default.Container container)
        {
            
            int id;
            bool wantExist = false;

            Mission mission=null;
            //Ввод и валидация Id существующей задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter a Id of the task which you need to edit: ");
                string userInput = Console.ReadLine();
                if (userInput == "-exit") { wantExist = true; break; }
                if (Int32.TryParse(userInput, out id)) {
                    
                    try
                    {
                        mission = container.Missions.Where(c => c.Id == id).FirstOrDefault();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("ok");
                    }
                    catch { }
                }

                if (mission!=null)
                {
                    DisplayMission(mission);
                    Console.WriteLine("Task is find! Input new property ('skip' to skip)");
                    Console.WriteLine();
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Task with id = {id} is not exist");
                    Console.WriteLine();
                }
            }

        

            

            //Ввод и валидация имени новой задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Task name (max 30 characters): ");
                string userInput = Console.ReadLine();                
                if (userInput == "-exit") { wantExist = true; break; }
                if (userInput == "skip") {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("is not changed");
                    break;
                }
                mission.Name = userInput;

                if (mission.Name.Length <= 30 && mission.Name.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exceeded the maximum length 30 or Empty");
                    Console.WriteLine();
                }
            }

            //Ввод и валидация описания новой задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Task description (max 300 characters): ");
                string userInput = Console.ReadLine();
                if (userInput == "-exit") { wantExist = true; break; }
                if (userInput == "skip") {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("is not changed");
                    break;
                }

                mission.Description = userInput;
                if (mission.Description.Length <= 300)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exceeded the maximum length 300");
                    Console.WriteLine();
                }
            }

            //Ввод и валидация дедлайна новой задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Deadline (DD-MM-YYYY hh:mm). Blank if not needed: ");
                string userInput = Console.ReadLine();
                if (userInput == "-exit") { wantExist = true; break; }
                if (userInput == "skip") {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("is not changed");
                    break;
                }
                
                if (!String.IsNullOrEmpty(userInput))
                {
                    try
                    {
                        mission.Deadline = DateTime.ParseExact(userInput, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("ok");
                        break;
                    }
                    catch
                    {

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Incorrect date time format");
                        Console.WriteLine();
                    }
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    break;
                }
            }

            if (!wantExist)
            {
                container.UpdateObject(mission);               

                var response= container.SaveChanges();
                //Тут должна быть проверка ответа сервера
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(new string('-', 20));
                Console.WriteLine("Задача сохранена");
            }
            return wantExist;
        }

        //Отправка запроса на сервер на редактирование задачи
        private static bool ChangeStatusMission(Default.Container container)
        {

            int id;
            bool wantExist = false;

            Mission mission = new Mission();
            
            //Ввод и валидация Id существующей задачи
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter a Id of the task which you need to change status: ");
                string userInput = Console.ReadLine();
                if (userInput == "-exit") { wantExist = true; break; }
                if (Int32.TryParse(userInput, out id))
                    try
                    {
                        mission = container.Missions.Where(c => c.Id == id).FirstOrDefault();                       
                   }
                    catch { } 

                if (mission != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Task with id = {id} is not exist");
                    Console.WriteLine();
                }
            }

           

            Console.WriteLine("Task is find! Input new status ('2' to cancel, '3' to finished)");
            Console.WriteLine();


            //Ввод и валидация имени нового статуса
            while (!wantExist)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("new Status : ");
                string userInput= Console.ReadLine();
                if (userInput == "-exit") { wantExist = true; break; }
                byte statusCode;
                bool isnumber = Byte.TryParse(userInput, out statusCode);

                if (isnumber && (statusCode==2 || statusCode==3))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ok");
                    
                    
                    //Логика смены статуса. Должны быть перенесена в модель
                    if ((State)statusCode == State.Canceled && mission.TaskState == State.Waiting) {
                        mission.TaskState = State.Canceled;
                    }
                    if ((State)statusCode == State.Canceled && mission.TaskState == State.Finished) { mission.TaskState = State.Waiting; }
                    if ((State)statusCode == State.Finished ) { mission.TaskState = State.Finished; }
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Status is not recognized");
                    Console.WriteLine();
                }
            }

            if (!wantExist)
            {
                container.UpdateObject(mission);
                var response = container.SaveChanges();
                //Тут должна быть проверка ответа сервера
                //........................
                container.SaveChanges();
            }
            return wantExist;
            
        }

        //Отправка запроса на сервер на удаление задачи
        private static void DeleteMission(Default.Container container)
        {
            Console.WriteLine("Enter a name of the contact which you need to delete:");
            int id;
            if (!Int32.TryParse(Console.ReadLine(), out id)) { return; }

            Mission mission = container.Missions.Where(c => c.Id == id).FirstOrDefault();
            if (mission != null)
            {
                container.DeleteObject(mission);
                var serviceResponse = container.SaveChanges();
                foreach (var operationResponse in serviceResponse)
                {
                    Console.WriteLine("Response: {0}", operationResponse.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Object is not found.");
            }
        }

        //Ввод номера Localhost и попытка соединения
        private static void ConnectToServer(ref string correctServiceUri)
        {
            bool isConnect = false;
            
            //подключения
            while (!isConnect)
            {
                HttpClient client = new HttpClient();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter the localhost port number: ");
                string portn = Console.ReadLine();

                string serviceUri = $"http://localhost:{portn}";
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Выполняется попытка подключения..");
                try
                {
                    var response = client.GetAsync($"{serviceUri}/Home/TestConnection").Result;
                    Boolean.TryParse(response.Content.ReadAsStringAsync().Result, out isConnect);
                    correctServiceUri = serviceUri;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Соединение установлено");
                    Console.WriteLine();
                    isConnect = true;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка соединения");
                }
                
            }
            
        }

        

       
    }
}
