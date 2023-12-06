using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using ProyectoRedes.Models;
using System;
using System.Net;
using System.Numerics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using Microsoft.Net.Http.Headers;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Debugger.Contracts.EditAndContinue;

namespace ProyectoRedes.Controllers
{
    public class GameController : Controller
    {


        // GET: GameController
        public ActionResult Index(
            List<string> players,
            List<string> enemies,
            string leader,
            string result,
            string phase,
            string status,
            List<string> group,
            List<bool> votes,
            int decades,
            string name, string gameId, string password, string owner, GameEnded gameEnded, string server, List<Rounds> rounds, string gameName)
        {


            if (players.Count > 0)
            {
                if (enemies.Count > 0)
                {
                    var player = new Player();
                    var others = new List<PlayerCheck>();
                    var delete = new List<string>();
                    delete = players;
                    foreach (var p in players)
                    {
                        foreach (var enemy in enemies)
                        {
                            if (enemy == p)
                            {
                                var other = new PlayerCheck();
                                other.name = enemy;
                                other.isEnemy = true;
                                others.Add(other);

                            }

                        }

                    }

                    foreach (var enemy in others)
                    {
                        delete.Remove(enemy.name);

                    }

                    foreach (var p in players)
                    {
                        var other = new PlayerCheck();
                        other.name = p;
                        other.isEnemy = false;
                        others.Add(other);
                    }



                    player.player = name;

                    player.otherPlayers = others;

                    if (leader != null &&
                        result != null &&
                         phase != null &&
                         status != null &&
                         group != null &&
                         votes != null) {

                        player.leader = leader;
                        player.group = group;
                        player.status = status;
                        player.votes = votes;

                    }
                    player.decade = decades;
                    player.gameId = gameId;
                    player.password = password;

                    player.server = server;
                    player.owner = owner;
                    player.gameName = gameName;
                    if (decades > 0)
                    {
                        player.rounds = GetRounds(gameId, name, password, server);
                        player.gameEnded = GetRoundsWinner(gameId, password, name, server);
                    }
                    else
                    {
                        player.rounds = rounds;
                        player.gameEnded = gameEnded;
                    }

                    return View(player);

                }
                else
                {
                    var player = new Player();
                    var otherPlayers = new List<PlayerCheck>();
                    foreach (var p in players)
                    {
                        var other = new PlayerCheck();
                        other.name = p;
                        other.isEnemy = false;
                        otherPlayers.Add(other);
                    }
                    player.player = name;
                    player.otherPlayers = otherPlayers.ToList();
                    if (leader != null &&
                       result != null &&
                        phase != null &&
                        status != null &&
                        group != null &&
                        votes != null)
                    {

                        player.leader = leader;
                        player.group = group;
                        player.status = status;
                        player.votes = votes;

                    }
                    player.decade = decades;
                    player.gameId = gameId;
                    player.password = password;
                    player.gameName = gameName;
                    player.owner = owner;
                    if (decades != 0)
                    {
                        player.rounds = GetRounds(gameId, name, password, server);
                        player.gameEnded = GetRoundsWinner(gameId, password, name, server);
                    }
                    else
                    {
                        player.rounds = rounds;
                        player.gameEnded = gameEnded;
                    }

                 

                    player.server = server;

                    return View(player);
                }


            } else {
                return View();
            }
        }


        public ActionResult GetGame(string server, string player)
        {
            TempData["Server"] = server;
            TempData["Player"] = player;
            return View();

        }

        // GET
        [HttpPost]
        public ActionResult GetGame(GetGame getGame, string server, string username)
        {
            try
            {
                    using (var client = new HttpClient())
                    {
                        TempData["Server"] = server;
                        TempData["Player"] = username;
                    string url;

                    if (getGame.name == null && getGame.status == null && getGame.limit == null && getGame.page == null)//Cuando esta nulo todo
                    {
                        url = server + "/api/games";

                    }
                    else if (getGame.name == null && getGame.status != null && getGame.limit != null && getGame.page != null)//Nombre nulo
                    {
                        url = server + "/api/games?" + 
                         "status=" + getGame.status + "&page=" + getGame.page;
                    }
                    else if (getGame.name != null && getGame.status != null && getGame.limit == null && getGame.page != null)//Limite nulo
                    {
                        url = server + "/api/games?name=" + getGame.name
                        + "&status=" + getGame.status + "&page=" + getGame.page;
                    }
                    else if (getGame.name != null && getGame.status != null && getGame.limit != null && getGame.page == null)//Page nulo
                    {
                        url = server + "/api/games?name=" + getGame.name
                        + "&status=" + getGame.status + "&limit=" + getGame.limit;
                    }
                    else if(getGame.name == null && getGame.status == null && getGame.limit != null && getGame.page != null)//Status nulo
                    {
                        url = server + "/api/games?name=" + getGame.name
                           + "&page=" + getGame.page + "&limit=" + getGame.limit;
                    }else if (getGame.name == null && getGame.status == null && getGame.limit != null && getGame.page != null)//Si el name y el status son nulos
                    {
                        url = server + "/api/games?page=" + getGame.page + "&limit=" + getGame.limit;
                    }else if (getGame.name != null && getGame.status == null && getGame.limit == null && getGame.page == null)
                    {
                        url = server + "/api/games?name=" + getGame.name;
                    }else if (getGame.name == null && getGame.status != null && getGame.limit == null && getGame.page == null)
                    {
                        url = server + "/api/games?" 
                         + "status=" + getGame.status;
                    }else if (getGame.name == null && getGame.status == null && getGame.limit != null && getGame.page == null)
                    {
                        url = server + "/api/games?" + "limit=" + getGame.limit;
                    }else if (getGame.name == null && getGame.status == null && getGame.limit == null && getGame.page != null)
                    {
                        url = server + "/api/games?" + "page=" + getGame.page;
                    }
                    else //Cuando no esta nulo
                    {
                        url = server + "/api/games?name=" + getGame.name
                          + "&status=" + getGame.status + "&page=" + getGame.page + "&limit=" + getGame.limit;
                    }
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                        var result = responseTask.Result;

                        if (result.IsSuccessStatusCode)
                        {
                            //var data = JsonConvert.DeserializeObject<Data>(result);
                            var readTask = result.Content.ReadFromJsonAsync<Data>();
                            readTask.Wait();

                            var data = readTask.Result;
                            //ViewBag.Message = data.ToJson();


                            //var players = data.data.ToList();
                            ViewBag.Data = data;
                            //TempData["Data"] = data;
                       
                            return View();
                        }
                        else
                        {
                            // Manejo de errores aquí, por ejemplo, puedes registrar el código de estado y el mensaje de error.
                            var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                            var error = readTask.Result;

                            var msg = error.msg;
                            ViewBag.Message = msg;
                            // return RedirectToAction("Options", new { msg});
                            return View();
                      
                        }
                    }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                // return RedirectToAction("Options", new { msg});
                var msg = ex.Message;

                return RedirectToAction("Index", "Home", new { msg = msg });
            }
        }

        public ActionResult ShowGames(Data data,string server)
        {

            TempData["Server"] = server;
            return View(data);
        }
        

        [HttpPost]
        public IndexReturn ShowRoundsMethod(string gameId, string roundId, string password, string name, string server)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Set the base address
                    string baseUrl = server + "/api/games/" + gameId + "/rounds/" + roundId;


                    // Crear una solicitud HTTP POST con el cuerpo JSON
                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);

                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", name);
                    }
                    else
                    {
                        request.Headers.Add("player", name);
                    }


                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);
                    // Send the GET request with headers and query parameter

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        // Handle the successful response here
                        var readTask = result.Content.ReadFromJsonAsync<DataRounds>();
                        readTask.Wait();
                        var data = readTask.Result;
                        var leader = data.data.leader;
                        var status = data.data.status;
                        var resultRound = data.data.result;
                        var phase = data.data.phase;
                        var group = data.data.group;
                        var votes = data.data.votes;

                        IndexReturn resp = new IndexReturn();
                        resp.phase = phase;
                        resp.group = group;
                        resp.result = resultRound;
                        resp.status = status;
                        resp.leader = leader;
                        resp.votes = votes;

                        return resp;

                    }
                    else
                    {
                        return null;
                    }
                }
            } catch (Exception ex)
            {
                return null;
            }
        }
        public string getGameOwner(string nameGame, string status, string server)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    var responseTask = client.GetAsync(server + "/api/games?name=" + nameGame + "&status=" + status);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //var data = JsonConvert.DeserializeObject<Data>(result);
                        var readTask = result.Content.ReadFromJsonAsync<Data>();
                        readTask.Wait();

                        var data = readTask.Result;

                        return data.data[0].owner;
                    }
                    else
                    {
                        // Manejo de errores aquí, por ejemplo, puedes registrar el código de estado y el mensaje de error.
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        // return RedirectToAction("Options", new { msg});
                        return "";
                    }

                }

            }
            catch (Exception e)
            {
                return "";
            }

        }
        public ActionResult GetGameMethod(string gameId, string name, string password, string server)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //    WebClient webClient = new WebClient();


                    string baseUrl = server + "/api/games/" + gameId;
                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);


                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", name);
                    }
                    else
                    {
                        request.Headers.Add("player", name);
                    }


                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadFromJsonAsync<DataGet>();
                        readTask.Wait();
                        var data = readTask.Result;
                        ViewBag.response = data.ToJson();

                        var players = data.data.players.ToList();
                        var enemies = data.data.enemies.ToList();
                        var roundId = data.data.currentRound;
                        var status = data.data.status;
                        var gameName = data.data.name;

                        var owner = getGameOwner(gameName, status, server);

                        if (owner == null)
                        {
                            owner = "";
                        }


                        IndexReturn resp = new IndexReturn();
                        List<Rounds> rounds = new List<Rounds>();
                        int decades = 0;
                        resp = ShowRoundsMethod(gameId, roundId, password, name, server);
                        GameEnded gameEnded = new GameEnded();
                        gameEnded = GetRoundsWinner(gameId, password, name, server);


                        if (resp != null)
                        {
                            if (gameEnded != null)
                            {

                                decades = GetRoundsMethod(gameId, password, name, decades, server);
                                rounds = GetRounds(gameId, name,password,server);
                                return RedirectToAction(nameof(Index),
                               new { players, enemies, resp.leader, resp.result, resp.phase, resp.status, resp.group, resp.votes, decades, name, gameId, password, owner, gameEnded, server, rounds, gameName });
                            }
                            else
                            {
                                GameEnded gameEnded1 = new GameEnded();
                                decades = GetRoundsMethod(gameId, password, name, decades,server);
                                rounds = GetRounds(gameId, name, password, server);
                                return RedirectToAction(nameof(Index),
                               new { players, enemies, resp.leader, resp.result, resp.phase, resp.status, resp.group, resp.votes, decades, name, gameId, password, owner, gameEnded, server, rounds, gameName });
                            }

                        }
                        else
                        {
                            GameEnded gameEnded1 = new GameEnded();
                            IndexReturn resp1 = new IndexReturn();
                            decades = GetRoundsMethod(gameId, password, name, decades,server);
                            return RedirectToAction(nameof(Index),
                               new { players, enemies, resp1.leader, resp1.result, resp1.phase, resp1.status, resp1.group, resp1.votes, decades, name, gameId, password, owner, gameEnded1, server, gameName });
                        }


                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        // return RedirectToAction("Options", new { msg});
                        return View();
                    }

                }
            }
            catch (Exception e)
            {


                var msg = e.Message;
                ViewBag.Message = msg;
                // return RedirectToAction("Options", new { msg});
                return RedirectToAction("Index", "Home", new { msg = msg });
            }


        }
        public ActionResult Create(string username, string server)
        {
            TempData["Username"] = username;
            TempData["Server"] = server;
            return View();
        }
        // POST: GameController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateGame game, string server)
        {
            try
            {

                using (var cliente = new HttpClient())
                {
                    TempData["Server"] = server;
                    var result = new HttpResponseMessage();
                    string url = server + "/api/games";
                    if (string.IsNullOrEmpty(game.password)) {
                        var requestBody = new
                        {
                            name = game.name,
                            owner = game.owner
                        };


                        // Serializar el objeto a JSON
                        string jsonBody = JsonConvert.SerializeObject(requestBody);

                        // Crear una solicitud HTTP POST con el cuerpo JSON
                        var request = new HttpRequestMessage(HttpMethod.Post, url)
                        {
                            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                        };
                        // Solicitud HTTP
                        var responseTask = cliente.SendAsync(request);
                        // Send the GET request with headers and query parameter

                        responseTask.Wait();
                        result = responseTask.Result;
                    }
                    else
                    {
                        var requestBody = new
                        {
                            name = game.name,
                            owner = game.owner,
                            password = game.password
                        };


                        // Serializar el objeto a JSON
                        string jsonBody = JsonConvert.SerializeObject(requestBody);

                        // Crear una solicitud HTTP POST con el cuerpo JSON
                        var request = new HttpRequestMessage(HttpMethod.Post, url)
                        {
                            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                        };
                        // Solicitud HTTP
                        var responseTask = cliente.SendAsync(request);
                        // Send the GET request with headers and query parameter

                        responseTask.Wait();
                        result = responseTask.Result;

                    }


                    //Globals.playerName = game.name;
                    //Globals.password = game.password;

                    if (result.IsSuccessStatusCode)
                    {


                        var readTask = result.Content.ReadFromJsonAsync<DataGet>();
                        readTask.Wait();
                        var data = readTask.Result;
                        ViewBag.message = data.ToJson();
                        var players = data.data.players.ToList();
                        var enemies = data.data.enemies.ToList();
                        var name = game.owner;
                        var owner = game.owner;
                        var password = game.password;
                        var gameId = data.data.id;
                        var gameName = data.data.name;


                        IndexReturn resp = new IndexReturn();

                        ViewBag.messageGood = "Juego Creado";
                        int decades = 0;
                        TempData["OwnerGame"] = owner;
                        TempData.Keep("OwnerGame");


                        return RedirectToAction(nameof(Index),
                           new { players, enemies, resp.leader, resp.result, resp.phase, resp.status, resp.group, resp.votes, decades, name, gameId, password, owner, server, gameName });

                    }
                    else
                    {
                        // Manejo de errores aquí, por ejemplo, puedes registrar el código de estado y el mensaje de error.
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        return View();
                        // return RedirectToAction("Options", new { msg});
                        
                    }
                }
            }
            catch (Exception error)
            {
                var msg = error.Message;
                ViewBag.Message = msg;
                // return RedirectToAction("Options", new { msg});
                return RedirectToAction("Index", "Home", new { msg = msg });
            }
        }

        public ActionResult StartGame(string gameId, string player, string password, string server)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    string baseUrl = server + "/api/games/" + gameId + "/start";

                    var request = new HttpRequestMessage(HttpMethod.Head, baseUrl);

                    request.Headers.Add("accept", "*/*");
                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", player);
                    }
                    else
                    {
                        request.Headers.Add("player", player);
                    }

                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {

                        //var data = JsonConvert.DeserializeObject<Data>(result);

                        ViewBag.response = "El juego ha comenzado!" + result.StatusCode;


                        var name = player;
                        // GetGameMethod(gameId,player,password);
                        return RedirectToAction("GetGameMethod", new { gameId, name, password, server });


                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;

                        return null;

                    }


                }
            }catch(Exception ex)
            {

                var msg = ex.Message;
                ViewBag.Message = msg;
                // return RedirectToAction("Options", new { msg});
                return RedirectToAction("Index", "Home", new { msg = msg });

            }
        }
        public ActionResult GetRounds()
        {
            return View();
        }

        // GET
        [HttpPost]
        public List<Rounds> GetRounds(string gameId, string player, string password, string server)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    string baseUrl = server + "/api/games/" + gameId + "/rounds";

                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);

                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", player);
                    }
                    else
                    {
                        request.Headers.Add("player", player);
                    }


                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    List<Rounds> round = new List<Rounds>();
                    if (result.IsSuccessStatusCode)
                    {
                        //var data = JsonConvert.DeserializeObject<Data>(result);
                        var readTask = result.Content.ReadFromJsonAsync<GetRounds>();
                        readTask.Wait();
                        var data = readTask.Result;


                        ViewBag.response = data.ToJson();

                        int i = 1;

                        foreach (var r in data.data)
                        {
                            Rounds round1 = new Rounds();
                            round1.round = i;
                            round1.result = r.result;
                            i++;
                            round.Add(round1);
                        }

                        return round;

                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        // return RedirectToAction("Options", new { msg});
                        return null;
                    }


                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public int GetRoundsMethod(string gameId, string password, string name, int decadeNum, string server)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string baseUrl = server + "/api/games/" + gameId + "/rounds";

                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);

                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", name);
                    }
                    else
                    {
                        request.Headers.Add("player", name);
                    }

                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //var data = JsonConvert.DeserializeObject<Data>(result);
                        var readTask = result.Content.ReadFromJsonAsync<GetRounds>();
                        readTask.Wait();
                        var data = readTask.Result;
                        decadeNum = data.data.Count();

                        ViewBag.response = data.ToJson();
                        return decadeNum;

                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        // return RedirectToAction("Options", new { msg});
                        return 0;
                    }


                }
            }catch(Exception ex)
            {
                return 0;
            }
        }

        public GameEnded GetRoundsWinner(string gameId, string password, string playerName, string server)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    GameEnded game = new GameEnded();

                    string baseUrl = server + "/api/games/" + gameId + "/rounds";

                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);

                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", playerName);
                    }
                    else
                    {
                        request.Headers.Add("player", playerName);
                    }

                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //var data = JsonConvert.DeserializeObject<Data>(result);
                        var readTask = result.Content.ReadFromJsonAsync<GetRounds>();
                        readTask.Wait();
                        var data = readTask.Result;
                        //decadeNum = data.data.Count();

                        ViewBag.response = data.ToJson();

                        int citizens = 0;
                        int badGuys = 0;


                        foreach (var round in data.data)
                        {
                            if (round.result == "enemies")
                            {
                                badGuys++;
                            }

                            if (round.result == "citizens")
                            {
                                citizens++;
                            }
                        }

                        if (badGuys >= 3)
                        {
                            game.ended = true;
                            game.whoWin = "enemies";
                            return game;
                        }

                        if (citizens >= 3)
                        {
                            game.ended = true;
                            game.whoWin = "citizens";
                            return game;
                        }

                        return null;

                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        // return RedirectToAction("Options", new { msg});
                        return null;
                    }


                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult JoinGame(string id,string username, string server)
        {
            TempData["Id"] = id;
            TempData["Username"] = username;
            TempData["Server"] = server;
            return View();
        }

        // PUT : PlayerController/JoinGame
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinGame(JoinGame game, string server)
        {
            try
            {

                
                //if (player < 10) { 
                    using (var client = new HttpClient())
                    {
                        TempData["Server"] = server;
                        string baseUrl = server + "/api/games/" + game.id;
                        var requestBody = new
                        {
                            player = game.player
                        };
                        string jsonBody = JsonConvert.SerializeObject(requestBody);
                        var request = new HttpRequestMessage(HttpMethod.Put, baseUrl)
                        {
                            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                        };

                        if (string.IsNullOrEmpty(game.password))
                        {

                            request.Headers.Add("player", game.player);
                        }
                        else
                        {
                            request.Headers.Add("password", game.password);
                            request.Headers.Add("player", game.player);
                        }

                        // Solicitud HTTP
                        var responseTask = client.SendAsync(request);
                        // Send the GET request with headers and query parameter

                        responseTask.Wait();

                        var result = responseTask.Result;

                        if (result.IsSuccessStatusCode)
                        {
                            // Handle the successful response here
                            var readTask = result.Content.ReadFromJsonAsync<DataGet>();
                            readTask.Wait();
                            var data = readTask.Result;

                            var players = data.data.players.ToList();
                            var enemies = data.data.enemies.ToList();
                            var name = game.player;
                            var gameId = data.data.id;
                            var password = game.password;
                            var gameName = data.data.name;

                            IndexReturn resp = new IndexReturn();
                            int decades = 0;
                            return RedirectToAction(nameof(Index),
                               new { players, enemies, resp.leader, resp.result, resp.phase, resp.status, resp.group, resp.votes, decades, name, gameId, password, server, gameName });
                        }
                        else
                        {
                            // Manejo de errores aquí, por ejemplo, puedes registrar el código de estado y el mensaje de error.
                            var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                            var error = readTask.Result;

                            var msg = error.msg;
                            ViewBag.Message = msg;

                            return View();

                        }
                    }
                //}
                //else
                //{
                //    var msg = "Lamentablemente, no podemos admitir a más jugadores, hemos llegado al límite permitido.";
                //    ViewBag.Message = msg;

                //    return View();
                //}
            }catch(Exception ex)
            {
                return RedirectToAction("Index", "Home", new { msg = ex.Message });
            }
        }

   
        public ActionResult Options(string msg, string player, string server )
        {
            TempData["Username"] = player;
            TempData["Server"] = server;
            ViewBag.messageBad = msg;
            return View();
        }


        public ActionResult OptionsName(IFormCollection form)
        {
            //Guardar nombre de usuario
            string username = form["username"];
            TempData["Username"] = username;

            string server = form["server"];
            TempData["Server"] = server;

            return View("Options");
        }
  
      

        [HttpGet]
        public ActionResult ProposeGroupMethod(int decade, string gameId, string player, string password, string server)
        {
            var list = new ListGroup { 
                group = new List<string>(),
            };
            string roundId = GetRoundId(gameId, player, password,server);
            List<string> otherPlayers = GetPlayers(gameId, player, password, server);

            var model = new ModelGroup
            {
                groups = list.group,
                otherPlayers = otherPlayers,
                decade = decade,
                gameId = gameId,
                roundId = roundId,
                player = player,
                password = password,
                server = server
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ProposeGroupMethod(ModelGroup game)
        {
            try
            {
                HashSet<string> nombresUnicos = new HashSet<string>(game.groups);

                // Si la cantidad de elementos en el HashSet es igual a la cantidad de elementos en la lista original,
                // entonces no hay nombres repetidos.
                bool sinNombresRepetidos = nombresUnicos.Count == game.groups.Count;

                if (sinNombresRepetidos)
                {
                    using (var client = new HttpClient())
                    {
                        // Set the base address
                        string baseUrl = game.server + "/api/games/" + game.gameId + "/rounds/" + game.roundId;

                        List<string> g = game.groups;

                        var requestBody = new
                        {
                            group = g
                        };

                        // Serializar el objeto a JSON
                        string jsonBody = JsonConvert.SerializeObject(requestBody);

                        // Crear una solicitud HTTP POST con el cuerpo JSON
                        var request = new HttpRequestMessage(HttpMethod.Patch, baseUrl)
                        {
                            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                        };


                        if (!string.IsNullOrEmpty(game.password))
                        {
                            request.Headers.Add("password", game.password);
                            request.Headers.Add("player", game.player);
                        }
                        else
                        {
                            request.Headers.Add("player", game.player);
                        }

                        // Solicitud HTTP
                        var responseTask = client.SendAsync(request);
                        // Send the GET request with headers and query parameter

                        responseTask.Wait();

                        var result = responseTask.Result;

                        if (result.IsSuccessStatusCode)
                        {
                            // Handle the successful response here
                            var readTask = result.Content.ReadFromJsonAsync<GroupData>();
                            readTask.Wait();
                            var data = readTask.Result;

                            ViewBag.Message = data.ToJson();

                            var gameId = game.gameId;
                            var name = game.player;
                            var password = game.password;
                            var server = game.server;

                            return RedirectToAction(nameof(GetGameMethod), new { gameId, name, password, server });
                        }
                        else
                        {
                            var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                            var error = readTask.Result;

                            var msg = error.msg;
                            ViewBag.Message = msg;
                            // return RedirectToAction("Options", new { msg});
                            TempData["Error"] = msg;
                            var gameId = game.gameId;
                            var name = game.player;
                            var password = game.password;
                            var server = game.server;
                            return RedirectToAction(nameof(GetGameMethod), new { gameId, name, password, server });
                        }


                    }
                }
                else
                {
                    var msg = "No se puede agregar grupos iguales";
                    ViewBag.Message = msg;
                    // return RedirectToAction("Options", new { msg});
                    ViewBag.Message = msg;
                    // return RedirectToAction("Options", new { msg});
                    TempData["Error"] = msg;
                    var gameId = game.gameId;
                    var name = game.player;
                    var password = game.password;
                    var server = game.server;
                    return RedirectToAction(nameof(GetGameMethod), new { gameId, name, password, server });
                }
            }catch(Exception ex)
            {
                return RedirectToAction("Index", "Home", new { msg = ex.Message });
            }
        }
        public string GetRoundId(string gameId, string name, string password, string server)
        {
            try { 
                using (var client = new HttpClient())
                {
                    //    WebClient webClient = new WebClient();

                    string baseUrl = server + "/api/games/" + gameId;

                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);



                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", name);
                    }
                    else
                    {
                        request.Headers.Add("player", name);
                    }

                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //var data = JsonConvert.DeserializeObject<Data>(result);
                        var readTask = result.Content.ReadFromJsonAsync<DataGet>();
                        readTask.Wait();
                        var data = readTask.Result;
                        ViewBag.response = data.ToJson();

                        string roundId = data.data.currentRound;
                        return roundId;
                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        // return RedirectToAction("Options", new { msg});
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public List<string> GetPlayers(string gameId, string name, string password, string server)
        {
            try {
                using (var client = new HttpClient())
                {
                    //    WebClient webClient = new WebClient();

                    string baseUrl = server + "/api/games/" + gameId;

                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);



                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", name);
                    }
                    else
                    {
                        request.Headers.Add("player", name);
                    }


                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //var data = JsonConvert.DeserializeObject<Data>(result);
                        var readTask = result.Content.ReadFromJsonAsync<DataGet>();
                        readTask.Wait();
                        var data = readTask.Result;
                        ViewBag.response = data.ToJson();

                        List<string> num = data.data.players.ToList();
                        return num;
                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        ViewBag.Message = msg;
                        // return RedirectToAction("Options", new { msg});
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        
        //public ActionResult Vote()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(bool vote, string gameId, string player, string password, string server)
        {
            try { 
                string roundId = GetRoundId(gameId, player, password, server);
                using (var client = new HttpClient())
                {
                    // Set the base address
                    string baseUrl = server + "/api/games/" + gameId + "/rounds/" + roundId;


                    var requestBody = new
                    {
                        vote = vote
                    };


                    // Serializar el objeto a JSON
                    string jsonBody = JsonConvert.SerializeObject(requestBody);

                    // Crear una solicitud HTTP POST con el cuerpo JSON
                    var request = new HttpRequestMessage(HttpMethod.Post, baseUrl)
                    {
                        Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                    };


                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", player);
                    }
                    else
                    {
                        request.Headers.Add("player", player);
                    }


                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);
                    // Send the GET request with headers and query parameter

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        // Handle the successful response here
                        var readTask = result.Content.ReadFromJsonAsync<GroupData>();
                        readTask.Wait();
                        var data = readTask.Result;

                        ViewBag.Message = data.ToJson();

                        var name = player;

                        return RedirectToAction(nameof(GetGameMethod), new { gameId, name, password , server});
                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        TempData["Error"] = msg;
                        ViewBag.Message = msg;
                        var name = player;
                        return RedirectToAction(nameof(GetGameMethod), new { gameId, name, password, server });
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { msg = ex.Message });
            }
        }



    //public ActionResult Action()
    //{
    //    return View();
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Action(bool action, string gameId, string player, string password, string server)
    {
            try
            {
                string roundId = GetRoundId(gameId, player, password, server);
                using (var client = new HttpClient())
                {
                    // Set the base address
                    string baseUrl = server + "/api/games/" + gameId + "/rounds/" + roundId;


                    var requestBody = new
                    {
                        action = action
                    };

                    // Serializar el objeto a JSON
                    string jsonBody = JsonConvert.SerializeObject(requestBody);

                    // Crear una solicitud HTTP POST con el cuerpo JSON
                    var request = new HttpRequestMessage(HttpMethod.Put, baseUrl)
                    {
                        Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                    };

                    if (!string.IsNullOrEmpty(password))
                    {
                        request.Headers.Add("password", password);
                        request.Headers.Add("player", player);
                    }
                    else
                    {
                        request.Headers.Add("player", player);
                    }



                    // Solicitud HTTP
                    var responseTask = client.SendAsync(request);
                    // Send the GET request with headers and query parameter

                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        // Handle the successful response here
                        var readTask = result.Content.ReadFromJsonAsync<GroupData>();
                        readTask.Wait();
                        var data = readTask.Result;

                        ViewBag.Message = data.ToJson();

                        var name = player;

                        return RedirectToAction(nameof(GetGameMethod), new { gameId, name, password, server });
                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<MessageError>();

                        var error = readTask.Result;

                        var msg = error.msg;
                        TempData["Error"] = msg;
                        ViewBag.Message = msg;
                        var name = player;
                        return RedirectToAction(nameof(GetGameMethod), new { gameId, name, password, server });
                    }
                }
            }catch(Exception ex)
            {
                return RedirectToAction("Index", "Home", new { ex.Message });
            }

        }
}
}
