using API_ContaminaDOS.Data;
using API_ContaminaDOS.Models;
using API_ContaminaDOS.Models.DTO;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Numerics;
using System.Runtime.InteropServices;

namespace API_ContaminaDOS.Controllers
{
    
    [Route("api")]
    [ApiController]
    [ErrorActionFilter]
    public class GameController : ControllerBase
    {
        private ContaminaDOSContext db = new ContaminaDOSContext();

        [ApiExplorerSettings(IgnoreApi = true)]
        public void CreatePlayer(string gameId, string playerName)
        {
            try
            {
                Player player = new Player();
                Guid playerId = Guid.NewGuid();
                player.playerId = playerId.ToString();
                player.playerName = playerName;
                player.gameId = gameId;


                db.Players.Add(player);
                db.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }
        // POST CREATE GAME
        [Route("games")]
        [HttpPost]
        public ActionResult games([FromBody][Optional] RequestGame? requestGame)
        {   
            try
            {
                if (requestGame == null)
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The body can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(requestGame.name) || string.IsNullOrEmpty(requestGame.owner))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields name or owner can not be empty";
                    return StatusCode(400, error);
                }
        
                var gameFound = db.Games.Where(s => s.gameName == requestGame.name).ToList();
                if (gameFound.Count > 0)
                {
                    var error = new ErrorResponse();
                    error.status = 409;
                    error.msg = "Asset already exists";
                    return StatusCode(409, error);
                }
                else
                {


                    Game game = new Game();
                    Guid gameId = Guid.NewGuid();
                    game.gameId = gameId.ToString();
                    game.gameName = requestGame.name;
                    game.gameOwner = requestGame.owner;
                    DateTime now = DateTime.Now;
                    string fecha = now.ToString("yyyy-MM-dd HH:mm:ss");
                    game.createdAt = fecha;
                    game.updatedAt = fecha;
                    string cadena36Caracteres = "000000000000000000000000000000000000";
                    game.currentRound = cadena36Caracteres;
                    game.gameStatus = "lobby";

                    if (!string.IsNullOrEmpty(requestGame.password))
                    {
                        game.gamePassword = requestGame.password;

                        db.Games.Add(game);
                        db.SaveChanges();


                        CreatePlayer(game.gameId, requestGame.owner);

                        var playersName = new List<string>();
                        var playersList = db.Players.Where(s => s.gameId == game.gameId).ToList();

                        foreach (var item in playersList)
                        {
                            playersName.Add(item.playerName);
                        }

                        var enemies = new List<string>();

                        var data = new DataCreate();
                        data.id = game.gameId;
                        data.owner = requestGame.owner;
                        data.password = true;
                        data.name = game.gameName;
                        data.currentRound = game.currentRound;
                        data.enemies = enemies;
                        data.status = game.gameStatus;
                        data.players = playersName;
                        data.updatedAt = game.updatedAt;
                        data.createdAt = game.createdAt;

                        var response = new ResponseCreate();
                        response.status = 201;
                        response.msg = "Game Created";
                        response.data = data;

                        return StatusCode(201, response);
                    }
                    else
                    {
                        db.Games.Add(game);

                        db.SaveChanges();

                        CreatePlayer(game.gameId, requestGame.owner);

                        var enemies = new List<string>();

                        var data = new DataCreate();
                        var playersName = new List<string>();
                        var playersList = db.Players.Where(s => s.gameId == game.gameId).ToList();

                        foreach (var item in playersList)
                        {
                            playersName.Add(item.playerName);
                        }
                        data.owner = requestGame.owner;
                        data.id = game.gameId;
                        data.password = false;
                        data.name = game.gameName;
                        data.enemies = enemies;
                        data.status = game.gameStatus;
                        data.players = playersName;
                        data.updatedAt = game.updatedAt;
                        data.createdAt = game.createdAt;
                        data.currentRound = game.currentRound;

                        var response = new ResponseCreate();
                        response.status = 201;
                        response.msg = "Game Created";
                        response.data = data;

                        return StatusCode(201, response);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 400;
                error.msg = "Client Error";
                return StatusCode(400, error);
            }
        }

        [Route("games/{gameId?}/")]
        [HttpPut]
        public ActionResult join([FromBody] JoinRequest request, string? gameId)
        {
            
            try
            {
                string password = HttpContext.Request.Headers["password"];
                string name = HttpContext.Request.Headers["player"];

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(request.player))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The field PLayer can not be empty";
                    return StatusCode(400, error);
                }

                if (string.IsNullOrEmpty(gameId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields gameId can not be empty";
                    return StatusCode(400, error);
                }

                var game = db.Games.Find(gameId);
                if (game == null)
                {
                    var error = new ErrorResponse();
                    error.status = 404;
                    error.msg = "The specified resource was not found";
                    return StatusCode(404, error);
                }
                else
                {
                    var playerCount = db.Players.Where(s => s.gameId == game.gameId).ToList().Count();

                    if (playerCount <= 9)
                    {

                        if(game.gameStatus == "lobby")
                        {

                        

                        var playersList = db.Players.Where(s => s.gameId == game.gameId).ToList();
                        if (game.gamePassword == null)
                        {

                            bool equal = false;
                            foreach (var player in playersList)
                            {
                                if (request.player == player.playerName)
                                {
                                    equal = true;
                                }
                            }
                            if (equal)
                            {
                                var error = new ErrorResponse();
                                error.status = 409;
                                error.msg = "Asset already exists";
                                return StatusCode(409, error);
                            }
                            else
                            {


                                CreatePlayer(game.gameId, request.player);
                                var enemies = new List<string>();

                                var data = new DataCreate();
                                var playersName = new List<string>();

                                playersList = db.Players.Where(s => s.gameId == game.gameId).ToList();
                                foreach (var item in playersList)
                                {
                                    playersName.Add(item.playerName);
                                }
                                data.owner = game.gameOwner;
                                data.id = game.gameId;
                                data.password = false;
                                data.name = game.gameName;
                                data.enemies = enemies;
                                data.status = game.gameStatus;
                                data.currentRound = game.currentRound;
                                data.players = playersName;
                                data.updatedAt = game.updatedAt;
                                data.createdAt = game.createdAt;

                                var response = new ResponseCreate();
                                response.status = 201;
                                response.msg = "Joined Succesfully";
                                response.data = data;
                                return StatusCode(201, response);
                            }
                        }
                        else
                        {
                            if (game.gamePassword != password)
                            {
                                var error = new ErrorResponse();
                                error.status = 401;
                                error.msg = "Invalid credentials";
                                return StatusCode(401, error);
                            }
                            else
                            {

                                bool equal = false;
                                foreach (var player in playersList)
                                {
                                    if (request.player == player.playerName)
                                    {
                                        equal = true;
                                    }
                                }
                                if (equal)
                                {
                                    var error = new ErrorResponse();
                                    error.status = 409;
                                    error.msg = "Asset already exists";
                                    return StatusCode(409, error);
                                }
                                else
                                {
                                    CreatePlayer(game.gameId, request.player);
                                    var enemies = new List<string>();

                                    var data = new DataCreate();
                                    var playersName = new List<string>();

                                    playersList = db.Players.Where(s => s.gameId == game.gameId).ToList();
                                    foreach (var item in playersList)
                                    {
                                        playersName.Add(item.playerName);
                                    }
                                    data.owner = game.gameOwner;
                                    data.id = game.gameId;
                                    data.password = true;
                                    data.name = game.gameName;
                                    data.enemies = enemies;
                                    data.status = game.gameStatus;
                                    data.players = playersName;
                                    data.updatedAt = game.updatedAt;
                                    data.createdAt = game.createdAt;

                                    var response = new ResponseCreate();
                                    response.status = 201;
                                    response.msg = "Joined Succesfully";
                                    response.data = data;
                                    return StatusCode(201, response);
                                }
                            }
                        }
                        }
                        else
                        {
                            var error = new ErrorResponse();
                            error.status = 428;
                            error.msg = "This action is not allowed at this time";
                            return StatusCode(428, error);
                        }
                    }
                    else
                    {
                        var error = new ErrorResponse();
                        error.status = 401;
                        error.msg = "There are already 10 players";
                        return StatusCode(401, error);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("error=" + ex.Message);
            }

        }

        public record resp(string response);
        [ApiExplorerSettings(IgnoreApi = true)]
        public int getNum(int players)
        {
            Random random = new Random();

            int num = random.Next(players);
            return num;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        //Metodo para poner citizens y enemies
        private List<Player> classifyPlayers(List<Player> players)
        {
            var random = new Random();
            //La lista la hace random
            players = players.OrderBy(x => random.Next()).ToList();
            //Validacion por cantidad de personas
            if (players.Count() == 5 || players.Count() == 6)
            {//Si son 5 o 6 players solo 2 enemigos

                for (int i = 0; i < 2; i++)
                {
                    players[i].playerType = "enemy";
                }

                for (int i = 2; i < players.Count; i++)
                {
                    players[i].playerType = "citizen";
                }

            }
            else if (players.Count() == 7 || players.Count() == 8 || players.Count() == 9)
            {//Si son 7, 8 y 9 seran 3 enemigos
                for (int i = 0; i < 3; i++)
                {
                    players[i].playerType = "enemy";
                }

                for (int i = 3; i < players.Count; i++)
                {
                    players[i].playerType = "citizen";
                }
            }
            else if (players.Count() == 10)
            {//Si son 10 seran 4 enemigos
                for (int i = 0; i < 4; i++)
                {
                    players[i].playerType = "enemy";
                }

                for (int i = 4; i < players.Count; i++)
                {
                    players[i].playerType = "citizen";
                }
            }

            return players;
        }

        //HEAD START GAME
        [Route("games/{gameId?}/start")]
        [HttpHead]
        [ErrorActionFilter]
        public ActionResult Start(string? gameId, [FromHeader] string? password, [FromHeader] string? player)
        {
            try
            {
                //Validar el status que sea lobby
                if (string.IsNullOrEmpty(gameId))
                {
                    
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                    Response.Headers.Add("Status", "400");
                    Response.Headers.Add("X-msg", "The field gameId can not be empty");
                    return StatusCode(400);
                }
                if (string.IsNullOrEmpty(player))
                {
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                    Response.Headers.Add("Status", "400");
                    Response.Headers.Add("X-msg", "The fields player can not be empty");
                    return StatusCode(400);
                }

                var game = db.Games.Find(gameId);

                if (game != null)
                {
                    if (game.gameStatus == "lobby")
                    {
                        // Actualiza en la base de datos
                        if (game.gameOwner == player)
                        {
                            if (game.gamePassword == null)
                            {
                                var players = db.Players.Where(s => s.gameId == game.gameId).ToList();

                                if (players.Count() >= 5)
                                {

                                    //Devuelve la lista arreglada con todos los players classificados
                                    players = classifyPlayers(players);

                                    // Actualiza la base de datos con los cambios
                                    foreach (var p in players)
                                    {
                                        db.Players.Update(p);
                                    }


                                    int num = getNum(players.Count);
                                    //Añadir una nueva ronda
                                    var round = new Round();
                                    Guid roundId = Guid.NewGuid();
                                    round.roundId = roundId.ToString();
                                    round.gameId = game.gameId;
                                    round.leader = players[getNum(players.Count)].playerName;
                                    round.roundStatus = "waiting-on-leader";
                                    round.result = "none";
                                    round.phase = "vote1";
                                    db.Rounds.Add(round);

                                    game.gameStatus = "rounds";
                                    game.currentRound = roundId.ToString();

                                    db.Games.Update(game);

                                    db.SaveChanges();
                                    //Game started
                                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                    Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                                    Response.Headers.Add("Status", "200");
                                    Response.Headers.Add("X-msg", "Game Started");

                                    return StatusCode(200);
                                }
                                else
                                {
                                    //Jugadores Insuficentes
                                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                    Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                                    Response.Headers.Add("Status", "428");
                                    Response.Headers.Add("X-msg", "Need 5 players to start.");
                                    return StatusCode(428);
                                }

                            }
                            else
                            {

                                if (password == game.gamePassword)
                                {
                                    //Lista de players que tengan el id del juego
                                    var players = db.Players.Where(s => s.gameId == game.gameId).ToList();

                                    if (players.Count() >= 5)
                                    {

                                        //Devuelve la lista arreglada con todos los players classificados
                                        players = classifyPlayers(players);

                                        // Actualiza la base de datos con los cambios
                                        foreach (var p in players)
                                        {
                                            db.Players.Update(p);
                                        }


                                        int num = getNum(players.Count);
                                        //Añadir una nueva ronda
                                        var round = new Round();
                                        Guid roundId = Guid.NewGuid();
                                        round.roundId = roundId.ToString();
                                        round.gameId = game.gameId;
                                        round.leader = players[getNum(players.Count)].playerName;
                                        round.roundStatus = "waiting-on-leader";
                                        round.result = "none";
                                        round.phase = "vote1";
                                        db.Rounds.Add(round);

                                        game.gameStatus = "rounds";
                                        game.currentRound = roundId.ToString();

                                        db.Games.Update(game);

                                        db.SaveChanges();
                                        //Game started
                                        Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                        Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                                        Response.Headers.Add("Status", "200");
                                        Response.Headers.Add("X-msg", "Game Started");

                                        return StatusCode(200);
                                    }
                                    else
                                    {
                                        //Jugadores Insuficentes
                                        Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                        Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                                        Response.Headers.Add("Status", "428");
                                        Response.Headers.Add("X-msg", "Need 5 players to start.");
                                        return StatusCode(428);
                                    }
                                }
                                else
                                {
                                    //Acceso denegado
                                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                    Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                                    Response.Headers.Add("Status", "403");
                                    Response.Headers.Add("X-msg", "Forbidden.");
                                    return StatusCode(403);
                                }
                            
                            }
                        }
                        else
                        {//Falta de credenciales no autorizado
                            Response.Headers.Add("Access-Control-Allow-Origin", "*");
                            Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                            Response.Headers.Add("Status", "401");
                            Response.Headers.Add("X-msg", "Unauthorized");
                            return StatusCode(401);
                        }

                    }
                    else//No se puede ingresar por que ya inicio el juego
                    {
                        Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                        Response.Headers.Add("Status", "409");
                        Response.Headers.Add("X-msg", "Game already started.");
                        return StatusCode(409); 
                    }

                }
                else
                {//Juego no encontrado 404
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                    Response.Headers.Add("Status", "404");
                    Response.Headers.Add("X-msg", "Game does not exist");
                    // Devuelve una respuesta 404 Not Found sin contenido en el cuerpo
                    return NotFound();
                }
            }
            catch (Exception ex)
            {//Error del servidor 500
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
                Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
                Response.Headers.Add("X-msg", "Error: " + ex.Message);

                // Devuelve una respuesta 500 Internal Server Error sin contenido en el cuerpo
                return StatusCode(500);
            }
        }

        //METHOD CHECK PLAYERS
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool checkPlayer(string gameId, string name)
        {
            bool check = false;
            var player = db.Players.Where(s => s.gameId == gameId).ToList();
            foreach (var p in player)
            {
                if (p.playerName == name)
                {
                    check = true;
                }
            }

            return check;
        }
        //METHOD GET GROUPS
        [ApiExplorerSettings(IgnoreApi = true)]
        private List<string> getGroups(string gameId, string roundId)
        {
            var group = db.GroupRounds.Where(s => s.gameId == gameId && s.roundId == roundId).ToList();
            List<string> groups = new List<string>();

            foreach (var g in group)
            {
                groups.Add(g.player.playerName);
            }

            return groups;
            
        }
        //METHOD GET VOTES77
        [ApiExplorerSettings(IgnoreApi = true)]
        private List<bool> getVotes(string gameId, string roundId)
        {
            var vote = db.VoteRounds.Where(s => s.gameId == gameId && s.roundId == roundId).ToList();
            List<bool> votes = new List<bool>();

            foreach (var v in vote)
            {
                votes.Add(v.vote.Value);
            }

            return votes;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private bool getDecadeCorrect(int groupNumber, string roundId, string gameId)
        {
            var rounds = db.Rounds.Where(s => s.gameId == gameId).ToList().Count();
            var players = db.Players.Where(s => s.gameId == gameId).ToList().Count();
            var correctGroupDecade = false;

            if (rounds == 1)
            {
                if (
                    (players == 5 
                    || players == 6 
                    || players == 7)
                    && groupNumber == 2)
                {
                    correctGroupDecade = true;
                } else if (
                    (players == 8 ||
                    players == 9 ||
                    players == 10)
                    && groupNumber == 3) 
                { 
                    correctGroupDecade = true;
                }
            }
            else if (rounds == 2)
            {
                if (
                    (players == 5
                    || players == 6
                    || players == 7)
                    && groupNumber == 3)
                {
                    correctGroupDecade = true;
                }
                else if (
                    (players == 8 ||
                    players == 9 ||
                    players == 10)
                    && groupNumber == 4)
                {
                    correctGroupDecade = true;
                }
            }
            else if (rounds == 3)
            {
                if (
                    (players == 5)
                    && groupNumber == 2)
                {
                    correctGroupDecade = true;
                }
                else if ((players == 7) && groupNumber == 3)
                {
                    correctGroupDecade = true;
                }
                else if (
                    (players == 6 ||
                    players == 8 ||
                    players == 9 ||
                    players == 10)
                    && groupNumber == 4)
                {
                    correctGroupDecade = true;
                }
            }
            else if (rounds == 4)
            {
                if (
                    (players == 5 || players == 6)
                    && groupNumber == 3)
                {
                    correctGroupDecade = true;
                }
                else if ((players == 7) && groupNumber == 4)
                {
                    correctGroupDecade = true;
                }
                else if (
                    (players == 8 ||
                    players == 9 ||
                    players == 10)
                    && groupNumber == 5)
                {
                    correctGroupDecade = true;
                }
            }
            else if (rounds == 5)
            {
                if (
                    (players == 5)
                    && groupNumber == 3)
                {
                    correctGroupDecade = true;
                }
                else if ((players == 6 || players == 7) 
                    && groupNumber == 4)
                {
                    correctGroupDecade = true;
                }
                else if (
                    (players == 8 ||
                    players == 9 ||
                    players == 10)
                    && groupNumber == 5)
                {
                    correctGroupDecade = true;
                }
            }

            return correctGroupDecade;
        }
        //GET METHOD GET ROUNDS
        [Route("games/{gameId?}/rounds")]
        [HttpGet]
        [ErrorActionFilter]
        public ActionResult<IEnumerable<RoundsResponse>> GetRounds(string? gameId, [FromHeader] string? password, [FromHeader] string? player)
        {

            try
            {
                if (string.IsNullOrEmpty(gameId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields gameId can not be empty";
                    return StatusCode(400, error);
                }

                if (string.IsNullOrEmpty(player))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields player can not be empty";
                    return StatusCode(400, error);
                }
                //Revisa si existe el juego
                var game = db.Games.Find(gameId);
                if (game != null)
                {

                    if (checkPlayer(gameId, player))//Si checkPlayer es true
                    {
                        if (game.gamePassword == null)
                        {
                            var rounds = db.Rounds.Where(s => s.gameId == game.gameId).ToList();

                            var response = new RoundsResponse();
                            List<DataRounds> data = new List<DataRounds>();
                            foreach (var r in rounds)
                            {
                                DataRounds dataR = new DataRounds();
                                dataR.id = r.roundId;
                                dataR.leader = r.leader;
                                dataR.status = r.roundStatus;
                                dataR.phase = r.phase;
                                dataR.result = r.result;
                                dataR.group = getGroups(gameId, r.roundId);
                                dataR.votes = getVotes(gameId, r.roundId);

                                data.Add(dataR);
                            }

                            response.status = 200;
                            response.msg = "Rounds found";
                            response.data = data;
                            return StatusCode(200, response);//Todo correcto y sin fallos
                        }
                        else
                        {
                        if (game.gamePassword == password)
                        {
                            var rounds = db.Rounds.Where(s => s.gameId == game.gameId).ToList();

                            var response = new RoundsResponse();
                            List<DataRounds> data = new List<DataRounds>();
                            foreach (var r in rounds)
                            {
                                DataRounds dataR = new DataRounds();
                                dataR.id = r.roundId;
                                dataR.leader = r.leader;
                                dataR.status = r.roundStatus;
                                dataR.phase = r.phase;
                                dataR.result = r.result;
                                dataR.group = getGroups(gameId, r.roundId);
                                dataR.votes = getVotes(gameId, r.roundId);

                                data.Add(dataR);
                            }

                            response.status = 200;
                            response.msg = "Rounds found";
                            response.data = data;
                            return StatusCode(200, response);//Todo correcto y sin fallos
                        }
                        else
                        {//Credenciales invalidas
                            var error = new ErrorResponse();
                            error.status = 401;
                            error.msg = "Invalid credentials";
                            return StatusCode(401, error);
                        }
                    }
                           
                    }
                    else
                    {//Jugador no parte del juego
                        var error = new ErrorResponse();
                        error.status = 403;
                        error.msg = "Not part of the game";
                        return StatusCode(403, error);
                    }
                }
                else
                {//No hay juego
                    var error = new ErrorResponse();
                    error.status = 404;
                    error.msg = "Game does not exists";
                    return StatusCode(404, error);
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 400;
                error.msg = "Client Error";
                return StatusCode(400, error);
            }

        }


        //GET METHOD SHOW ROUND
        [Route("games/{gameId?}/rounds/{roundId?}")]
        [HttpGet]
        public ActionResult<IEnumerable<SRoundsResponse>> ShowRound(string? gameId,string? roundId, [FromHeader] string? password, [FromHeader] string? player)
        {

            try
            {
                if (string.IsNullOrEmpty(gameId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields gameId can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(roundId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields round id can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(player))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields player can not be empty";
                    return StatusCode(400, error);
                }
                //Revisa si existe el juego
                var game = db.Games.Find(gameId);
                
                if (game != null)
                {
                    var rounds1 = db.Rounds.Where(s => s.gameId == game.gameId && s.roundId == roundId).FirstOrDefault();
                    if (rounds1 == null)
                    {
                        var error = new ErrorResponse();
                        error.status = 404;
                        error.msg = "The specified resource was not found";
                        return StatusCode(404, error);
                    }
                    else
                    {

                    if (checkPlayer(gameId, player))//Si checkPlayer es true
                    {
                        if(game.gamePassword == null)
                        {
                            var rounds = db.Rounds.Where(s => s.gameId == game.gameId && s.roundId == roundId).ToList();

                            var response = new SRoundsResponse();
                            DataRounds dataR = new DataRounds();
                            foreach (var r in rounds)
                            {
                                dataR.id = r.roundId;
                                dataR.leader = r.leader;
                                dataR.status = r.roundStatus;
                                dataR.phase = r.phase;
                                dataR.result = r.result;
                                dataR.group = getGroups(gameId, r.roundId);
                                dataR.votes = getVotes(gameId, r.roundId);
                            }


                            response.status = 200;
                            response.msg = "Round found";
                            response.data = dataR;
                            return StatusCode(200, response);//Todo correcto y sin fallos
                        }
                        else
                        {

                        
                        if (game.gamePassword == password)
                        {
                            var rounds = db.Rounds.Where(s => s.gameId == game.gameId && s.roundId == roundId).ToList();

                            var response = new SRoundsResponse();
                            DataRounds dataR = new DataRounds();
                            foreach(var r in rounds)
                            {
                                dataR.id = r.roundId;
                                dataR.leader = r.leader;
                                dataR.status = r.roundStatus;
                                dataR.phase = r.phase;
                                dataR.result = r.result;
                                dataR.group = getGroups(gameId, r.roundId);
                                dataR.votes = getVotes(gameId, r.roundId);
                            }
                            
                            
                            response.status = 200;
                            response.msg = "Round found";
                            response.data = dataR;
                            return StatusCode(200, response);//Todo correcto y sin fallos
                        }
                        else
                        {//Credenciales invalidas
                            var error = new ErrorResponse();
                            error.status = 401;
                            error.msg = "Invalid credentials";
                            return StatusCode(401, error);
                        }
                        }

                    }
                    else
                    {//Jugador no parte del juego
                        var error = new ErrorResponse();
                        error.status = 403;
                        error.msg = "Not part of the game";
                        return StatusCode(403, error);
                    }
                }
            }
                else
                {//No hay juego
                    var error = new ErrorResponse();
                    error.status = 404;
                    error.msg = "The specified resource was not found";
                    return StatusCode(404, error);
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 500;
                error.msg = "Server Error";
                return StatusCode(500, error);
            }

        }

        //PATCH PROPOSE GROUP METHOD
        [Route("games/{gameId?}/rounds/{roundId?}")]
        [HttpPatch]
        public ActionResult ProposeGroup(string? gameId, string? roundId, [FromHeader] string? password, [FromHeader] string? player, [FromBody] GroupRequest group)
        {
            try
            {
                if (group.group.IsNullOrEmpty())
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The group can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(gameId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields gameId can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(roundId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields round id can not be empty";
                    return StatusCode(400, error);
                }

                if (string.IsNullOrEmpty(player))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields player can not be empty";
                    return StatusCode(400, error);
                }
                var game = db.Games.Find(gameId);
                if(game != null)
                {
                    if (getDecadeCorrect(group.group.Count(), roundId, gameId) == false)
                    {
                        var error = new ErrorResponse();
                        error.status = 404;
                        error.msg = "To many or few players";
                        return StatusCode(404, error);
                    }
                    else
                    {

                    
                    if (game.gamePassword != null)
                    {
                        if (game.gamePassword == password)
                        {

                            if (checkPlayer(gameId, player))
                            {

                                var currentRound = game.currentRound;
                                var round = db.Rounds.Where(s => s.roundId == currentRound).FirstOrDefault();
                                if (round.leader == player)
                                {
                                    
                                    
                                    foreach (var g in group.group)
                                    {
                                        if (checkPlayer(gameId, g))
                                        {
                                            GroupRound groupRound = new GroupRound();
                                            groupRound.roundId = currentRound;
                                            groupRound.gameId = gameId;
                                            groupRound.playerId = db.Players.Where(s => s.playerName == g && s.gameId == gameId).FirstOrDefault().playerId;
                                            db.GroupRounds.Add(groupRound);
                                        }
                                        else
                                        {//Not part of the game
                                                var error = new ErrorResponse();
                                                error.status = 403;
                                                error.msg = "Not part of the game";
                                                return StatusCode(403, error);

                                            }


                                    }//End foreach

                                    var rounds = db.Rounds.Where(s => s.gameId == game.gameId && s.roundId == roundId).FirstOrDefault();



                                    var response = new SRoundsResponse();
                                    DataRounds dataR = new DataRounds();


                                    if (rounds.roundStatus == "waiting-on-leader")
                                    {

                                        dataR.id = rounds.roundId;
                                        dataR.leader = rounds.leader;
                                        dataR.status = "voting";
                                        dataR.phase = rounds.phase;
                                        dataR.result = rounds.result;
                                        dataR.group = group.group;
                                        dataR.votes = getVotes(gameId, roundId);
                                    }
                                    else
                                    {//No se puedo 
                                        var error = new ErrorResponse();
                                        error.status = 428;
                                        error.msg = "This action is not allowed at this time";
                                        return StatusCode(428, error);
                                    }

                                    rounds.roundStatus = "voting";


                                    db.Rounds.Update(rounds);
                                    db.SaveChanges();

                                    response.status = 200;
                                    response.msg = "Group Created";
                                    response.data = dataR;
                                    return StatusCode(200, response);//Todo correcto y sin fallos

                                }
                                else
                                {//Youre not the leader
                                    var error = new ErrorResponse();
                                    error.status = 401;
                                    error.msg = "Invalid Credentials";
                                    return StatusCode(401, error);

                                }


                            }
                            else
                            {//Si el jugador no pertenece al juego
                                var error = new ErrorResponse();
                                error.status = 403;
                                error.msg = "Not part of the game";
                                return StatusCode(403, error);
                            }

                        }
                        else // el usuario dio contraseña incorrecta
                        {
                            var error = new ErrorResponse();
                            error.status = 401;
                            error.msg = "Invalid credentials";
                            return StatusCode(401, error);
                        }
                    }
                    else
                    { // el juego no tiene contraseña

                        if (checkPlayer(gameId, player))
                        {

                            var currentRound = game.currentRound;
                            var round = db.Rounds.Where(s => s.roundId == currentRound).FirstOrDefault();
                            if (round.leader == player)
                            {

                                var groupRoundF = new List<GroupRound>();
                                foreach (var g in group.group)
                                {
                                    if (checkPlayer(gameId, g))
                                    {
                                        GroupRound groupRound = new GroupRound();
                                        groupRound.roundId = currentRound;
                                        groupRound.gameId = gameId;
                                        groupRound.playerId = db.Players.Where(s => s.playerName == g && s.gameId == gameId).FirstOrDefault().playerId;
                                        db.GroupRounds.Add(groupRound);
                                    }
                                    else
                                    {//Not part of the game
                                        var error = new ErrorResponse();
                                        error.status = 404;
                                        error.msg = "The specified resource was not found";
                                        return StatusCode(404, error);

                                    }


                                }//End foreach




                                var rounds = db.Rounds.Where(s => s.gameId == game.gameId && s.roundId == roundId).FirstOrDefault();



                                var response = new SRoundsResponse();
                                DataRounds dataR = new DataRounds();


                                if (rounds.roundStatus == "waiting-on-leader")
                                {

                                    dataR.id = rounds.roundId;
                                    dataR.leader = rounds.leader;
                                    dataR.status = "voting";
                                    dataR.phase = rounds.phase;
                                    dataR.result = rounds.result;
                                    dataR.group = group.group;
                                    dataR.votes = getVotes(gameId, roundId);
                                }
                                else
                                {//No se puedo 
                                    var error = new ErrorResponse();
                                    error.status = 428;
                                    error.msg = "This action is not allowed at this time";
                                    return StatusCode(428, error);
                                }

                                rounds.roundStatus = "voting";


                                db.Rounds.Update(rounds);  
                                db.SaveChanges();

                                response.status = 200;
                                response.msg = "Group Created";
                                response.data = dataR;
                                return StatusCode(200, response);//Todo correcto y sin fallos

                            }
                            else
                            {//Youre not the leader
                                var error = new ErrorResponse();
                                error.status = 401;
                                error.msg = "Invalid Credentials";
                                return StatusCode(401, error);

                            }


                        }
                        else
                        {//Si el jugador no pertenece al juego
                            var error = new ErrorResponse();
                            error.status = 403;
                            error.msg = "Not part of the game";
                            return StatusCode(403, error);
                        }
                    }
                   }
                }
                else
                {//El juego no existe
                    var error = new ErrorResponse();
                    error.status = 404;
                    error.msg = "The specified resource was not found";
                    return StatusCode(404, error);
                }



                return null;
            
            }catch(Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 500;
                error.msg = "Server Error";
                return StatusCode(500, error);
            }
        }

        [Route("games")]
        [HttpGet]
        public ActionResult GetGames([FromQuery] string? name, string? status , int? page = 0, int? limit = 50)
        {
            try
            {
                if (limit == 0)
                {
                    var limitErrorResponse = new GameResponse
                    {
                        status = 400,
                        msg = "Invalid limit number",
                        data = null,
                        others = new Dictionary<string, string>()
                    };
                    return StatusCode(400, limitErrorResponse);
                }

                var gamesQuery = db.Games
                    .Include(g => g.Players)
                    .Where(g =>
                        (string.IsNullOrEmpty(name) || g.gameName == name) &&
                        (string.IsNullOrEmpty(status) || g.gameStatus == status));


                if (page == null)
                {
                    if (limit != null)
                    {
                        gamesQuery = gamesQuery.Take((int)limit);
                    }
                }
                else
                {
                    if (limit != null)
                    {
                        gamesQuery = gamesQuery.Skip((int)(page * limit)).Take((int)limit);
                    }
                }


                var games = gamesQuery.ToList();

                var gameData = games.Select(game => new GameSearch
                {
                    name = game.gameName,
                    owner = game.gameOwner,
                    status = game.gameStatus,
                    createdAt = game.createdAt,
                    updatedAt = game.updatedAt,
                    password = !string.IsNullOrEmpty(game.gamePassword),
                    players = game.Players.Select(p => p.playerName).ToList(),
                    enemies = game.Players
                        .Where(p => p.playerType == "enemy")
                        .Select(p => p.playerName)
                        .ToList(),
                    currentRound = game.currentRound,
                    id = game.gameId
                }).ToList();

                var response = new GameResponse
                {
                    status = 200,
                    msg = "Games found",
                    data = gameData,
                    others = new Dictionary<string, string>()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 400;
                error.msg = "Client Error";
                return StatusCode(400, error);
            }
        }

        [Route("games/{gameId}/rounds/{roundId}")]
        [HttpPut]
        public ActionResult SubmitAction(string? gameId, string? roundId, [FromHeader] string? password, [FromHeader] string? player, [FromBody] ActionRequest action)
        {
            try
            {
                if (string.IsNullOrEmpty(gameId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields gameId can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(roundId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields round id can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(player))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields player can not be empty";
                    return StatusCode(400, error);
                }
                if (action.action == null)
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The fields action can not be empty";
                    return StatusCode(400, error);
                }

                var currentGame = db.Games.Include(g => g.Players).FirstOrDefault(g => g.gameId == gameId);
                var currentRound = db.Rounds.Find(roundId);
                

                if (currentRound == null || currentGame == null)
                {
                    var error = new ErrorResponse();
                    error.status = 404;
                    error.msg = "The specified resource was not found";
                    return StatusCode(404, error);
                }

                if (currentRound.roundStatus != "waiting-on-group")
                {
                    var error = new ErrorResponse();
                    error.status = 428;
                    error.msg = "This action is not allowed at this time";
                    return StatusCode(428, error);
                }

                // Validación condicional
                string ifMatchHeader = Request.Headers["If-Match"]; // Obtiene el valor del encabezado If-Match

                if (string.IsNullOrWhiteSpace(ifMatchHeader) || ifMatchHeader == currentGame.updatedAt)
                {
                    //verifica que el jugador que envio el request si forme parte del juego
                    var currentPlayer = currentGame.Players.FirstOrDefault(p => p.playerName == player);

                    if (currentPlayer == null)
                    {
                        var error = new ErrorResponse();
                        error.status = 403;
                        error.msg = "Not part of the game";
                        return StatusCode(403, error);
                    }

                    //verifica que el jugador no vote dos veces en la misma partida

                    if (db.ActionRounds.Any(a => a.roundId == roundId && a.playerId == currentPlayer.playerId))
                    {
                        var error = new ErrorResponse();
                        error.status = 409;
                        error.msg = "Asset already exists";
                        return StatusCode(409, error);
                    }
                    //chequeo de que el jugador si pertenezca al grupo
                    var currentGroup = db.GroupRounds.Where(s => s.roundId==roundId).ToList();
                    bool insideGroup = false;
                    foreach(var group in currentGroup)
                    {
                        if (currentPlayer.playerId == group.playerId)
                        {
                            insideGroup = true;
                        }
                    }
                    if (!insideGroup)
                    {
                        var error = new ErrorResponse();
                        error.status = 403;
                        error.msg = "You can contribute";
                        return StatusCode(403, error);
                    }
                    if (currentPlayer.playerType == "citizen")
                    {
                        if(action.action == false)
                        {
                            var error = new ErrorResponse();
                            error.status = 403;
                            error.msg = "You cant sabotage, you are not a psicopath!";
                            return StatusCode(403, error);
                        }
                    }

                    //logica cuando el juego tiene contrasena
                    var players = db.Players.Where(s => s.gameId == gameId).ToList();
                    
                    if (currentGame.gamePassword != null)
                    {
                        if (currentGame.gamePassword == password)
                        {
                            //logica cuando tiene constrasena y ademas esta correcta
                            // Registra la acción en la base de datos
                            Guid idAction = Guid.NewGuid();

                            var newAction = new ActionRound
                            {
                                id = idAction.ToString(),
                                gameId = currentGame.gameId,
                                roundId = currentRound.roundId,
                                playerId = currentPlayer.playerId,
                                actionRound = action.action
                            };

                            db.ActionRounds.Add(newAction);
                            db.SaveChanges();

                            var group = new List<string>();



                            //Genera la respuesta
                            var actionResponse = new ActionResponse
                            {
                                status = 200,
                                msg = "Action registered",
                                data = new ActionData
                                {
                                    id = currentRound.roundId.ToString(),
                                    leader = currentRound.leader,
                                    status = currentRound.roundStatus,
                                    result = currentRound.result,
                                    phase = currentRound.phase,
                                    group = getGroups(gameId, roundId),
                                    votes = getVotes(gameId, roundId)    
                                }
                            };
                            checkActions(roundId, gameId, players);
                            return Ok(actionResponse);
                        }
                        else
                        {
                            var error = new ErrorResponse();
                            error.status = 401;
                            error.msg = "Invalid credentials";
                            return StatusCode(401, error);
                        }
                    }
                    //logica cuando el juego no tiene contrasena
                    else
                    {
                        Guid idAction = Guid.NewGuid();

                        var newAction = new ActionRound
                        {
                            id = idAction.ToString(),
                            gameId = currentGame.gameId,
                            roundId = currentRound.roundId,
                            playerId = currentPlayer.playerId,
                            actionRound = action.action
                        };

                        db.ActionRounds.Add(newAction);
                        db.SaveChanges();

                        //Genera la respuesta
                        var actionResponse = new ActionResponse
                        {
                            status = 200,
                            msg = "Action registered",
                            data = new ActionData
                            {
                                id = currentRound.roundId.ToString(),
                                leader = currentRound.leader,
                                status = currentRound.roundStatus,
                                result = currentRound.result,
                                phase = currentRound.phase,
                                group = getGroups(gameId, roundId),
                                votes = getVotes(gameId, roundId)
                            }
                        };

                        checkActions(roundId, gameId, players);
                        return Ok(actionResponse);
                    }
                }
                else
                {
                    var error = new ErrorResponse();
                    error.status = 428;
                    error.msg = "This action is not allowed at this time";
                    return StatusCode(428, error);
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 400;
                error.msg = "Client Error";
                return StatusCode(400, error);
            }
        }
        //Para ver quien gano y continuar en la ronda siguiente
        [ApiExplorerSettings(IgnoreApi = true)]
        private void checkActions(string roundId, string gameId, List<Player> players) {

            var group = db.GroupRounds.Where(s => s.roundId == roundId).ToList().Count();
            var groupAction = db.ActionRounds.Where(s => s.roundId == roundId).ToList().Count();

            if (group == groupAction)
            {

                var game = db.Games.Where(s => s.gameId == gameId).FirstOrDefault();

                var round = db.Rounds.Where(s => s.roundId == roundId).FirstOrDefault();

                var actions = db.ActionRounds.Where(s => s.roundId == roundId).ToList();

                var countBadActions = 0;


                foreach(var a in actions)
                {
                    if (a.actionRound == false)
                    {
                        countBadActions++;
                    }

                }

                if(countBadActions > 0)
                {
                    //crear una nueva ronda y dejar esta como ended y result enemies
                    round.result = "enemies";
                    round.roundStatus = "ended";
                    db.Rounds.Update(round);
                    db.SaveChanges();
                    string winners = checkWinners(gameId);
                    if (winners == null)
                    {
                        var roundNext = new Round();
                        Guid roundIdN = Guid.NewGuid();
                        roundNext.roundId = roundIdN.ToString();
                        roundNext.gameId = gameId;
                        roundNext.leader = players[getNum(players.Count)].playerName;
                        roundNext.roundStatus = "waiting-on-leader";
                        roundNext.result = "none";
                        roundNext.phase = "vote1";
                        DateTime now = DateTime.Now;
                        string fecha = now.ToString("yyyy-MM-dd HH:mm:ss");
                        game.updatedAt = fecha;
                        game.currentRound = roundIdN.ToString();


                        db.Games.Update(game);
                        db.Rounds.Add(roundNext);
                        db.SaveChanges();
                    }

                }
                else
                {
                    round.result = "citizens";
                    round.roundStatus = "ended";
                    db.Rounds.Update(round);
                    db.SaveChanges();
                    string winners = checkWinners(gameId);
                    if (winners == null)
                    {
                        var roundNext = new Round();
                        Guid roundIdN = Guid.NewGuid();
                        roundNext.roundId = roundIdN.ToString();
                        roundNext.gameId = gameId;
                        roundNext.leader = players[getNum(players.Count)].playerName;
                        roundNext.roundStatus = "waiting-on-leader";
                        roundNext.result = "none";
                        roundNext.phase = "vote1";
                        DateTime now = DateTime.Now;
                        string fecha = now.ToString("yyyy-MM-dd HH:mm:ss");
                        game.updatedAt = fecha;
                        game.currentRound = roundIdN.ToString();


                        db.Games.Update(game);
                        db.Rounds.Add(roundNext);
                        db.SaveChanges();

                    }


                }
            }
            else
            {
                //
            }


        }
        //Hecho por Milena
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool playerEnemy(string gameId, string name) //bool true si el jugador es enemigo
        {
            bool citizen = false;
            var player = db.Players.Where(s => s.gameId == gameId).ToList();
            foreach (var p in player)
            {
                if (p.playerName == name && p.playerType == "enemy")
                {
                    citizen = true;
                }
            }
            return citizen;
        }

        //Hecho por Milena
        [ApiExplorerSettings(IgnoreApi = true)]
        private List<string> getPlayers(string gameId) //Se obtiene una lista de los players del juego
        {
            var player = db.Players.Where(s => s.gameId == gameId).ToList();
            List<string> players = new List<string>();
            foreach (var p in player)
            {
                players.Add(p.playerName);
            }
            return players;
        }

        //Hecho por Milena
        [ApiExplorerSettings(IgnoreApi = true)]
        private List<string> getEnemies(string gameId, string name) //Devuelve la lista de enemigos pero solo para los player=enemy
        {
            var enemy = db.Players.Where(s => s.gameId == gameId && s.playerType == "enemy").ToList();
            List<string> enemies = new List<string>();
            if (playerEnemy(gameId, name))
            {
                foreach (var e in enemy)
                {
                    enemies.Add(e.playerName);
                }
            }
            return enemies;
        }

        //Hecho por Milena
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool playerG(string gameId, string name) //Bool true si el player pertenece al juego
        {
            bool playerG = false;
            var player = db.Players.Where(s => s.gameId == gameId).ToList();
            foreach (var p in player)
            {
                if (p.playerName == name)
                {
                    playerG = true;
                }
            }

            return playerG;
        }

        //Hecho por Milena
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool samePassword(string gameId, string password) //Bool true si el player pertenece al juego
        {
            bool sameP = false;
            var game = db.Games.Where(s => s.gameId == gameId).ToList();
            foreach (var g in game)
            {
                if (g.gamePassword == password)
                {
                    sameP = true;
                }
            }

            return sameP;
        }

        //Hecho por Milena
        [Route("games/{gameId?}/")]
        [HttpGet]
        public ActionResult<IEnumerable<ResponseGameId>> gameId(string? gameId, [FromHeader] string? password, [FromHeader] string? player)
        {
            try
            {
                if (string.IsNullOrEmpty(gameId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The field gameId can not be empty";
                    return StatusCode(400, error);
                }
                
                if (string.IsNullOrEmpty(player))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "Player field can not be empty";
                    return StatusCode(400, error);
                }

                var game = db.Games.Find(gameId);

                if (game == null)
                {
                    var error = new ErrorResponse();
                    error.status = 404;
                    error.msg = "The specified resource was not found";
                    return StatusCode(404, error);
                }
                else
                {

                    if (playerG(gameId, player))
                    {
                        if(game.gamePassword == null)
                        {
                            var games = db.Games.Where(s => s.gameId == game.gameId).ToList();
                            var response = new ResponseGameId();
                            DataGame dataGame = new DataGame();
                            List<string> players = getPlayers(gameId);
                            List<string> enemies = getEnemies(gameId, player);


                            foreach (var g in games)
                            {
                                dataGame.id = gameId;
                                dataGame.name = g.gameName;
                                dataGame.owner = g.gameOwner;
                                dataGame.status = g.gameStatus;
                                dataGame.password = g.gamePassword != null; //si es diferente de null  guarda true
                                dataGame.currentRound = g.currentRound;
                                dataGame.createdAt = g.createdAt;
                                dataGame.updatedAt = g.updatedAt;
                                dataGame.players = players;
                                dataGame.enemies = enemies;

                            }

                            response.status = 200;
                            response.msg = "Game Found";
                            response.data = dataGame;
                            return StatusCode(200, response);
                        }
                        else
                        {

                        

                        if (samePassword(gameId, password))
                        {
                            var games = db.Games.Where(s => s.gameId == game.gameId).ToList();
                            var response = new ResponseGameId();
                            DataGame dataGame = new DataGame();
                            List<string> players = getPlayers(gameId);
                            List<string> enemies = getEnemies(gameId, player);


                            foreach (var g in games)
                            {
                                dataGame.id = gameId;
                                dataGame.name = g.gameName;
                                dataGame.owner = g.gameOwner;
                                dataGame.status = g.gameStatus;
                                dataGame.password = g.gamePassword != null; //si es diferente de null  guarda true
                                dataGame.currentRound = g.currentRound;
                                dataGame.createdAt = g.createdAt;
                                dataGame.updatedAt = g.updatedAt;
                                dataGame.players = players;
                                dataGame.enemies = enemies;

                            }

                            response.status = 200;
                            response.msg = "Game Found";
                            response.data = dataGame;
                            return StatusCode(200, response);

                        }
                        else
                        {
                            var error = new ErrorResponse();
                            error.status = 401;
                            error.msg = "Invalid credentials";
                            return StatusCode(401, error);
                        }
                      }
                    }
                    else
                    {
                        var error = new ErrorResponse();
                        error.status = 403;
                        error.msg = "Not part of the game";
                        return StatusCode(403, error);
                    }

                }
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 404;
                error.msg = "Client Error";
                return StatusCode(404, error);
            }

        }

        //Hecho por Milena
        [ApiExplorerSettings(IgnoreApi = true)]
        private string gameState(string gameId) //
        {
            string state = "lobby";
            var game = db.Games.Where(s => s.gameId == gameId).ToList();
            foreach (var g in game)
            {
                if (g.gameStatus == "rounds")
                {
                    state = "rounds";
                }
                else
                    state = "ended";
            }
            return state;
        }

        //Hecho por Milena
        [ApiExplorerSettings(IgnoreApi = true)]
        private string idPlayer(string gameId, string name) //
        {
            string playerId = "";
            var player = db.Players.Where(s => s.gameId == gameId && s.playerName == name).ToList();
            foreach (var p in player)
            {
                playerId = p.playerId;
            }
            return playerId;
        }

        //METHOD GET GROUPS
        [ApiExplorerSettings(IgnoreApi = true)]
        private List<bool?> listVotes(string gameId, string roundId)
        {
            var vote = db.VoteRounds.Where(s => s.gameId == gameId && s.roundId == roundId).ToList();
            List<bool?> votes = new List<bool?>();

            foreach (var g in vote)
            {
                votes.Add(g.vote);
            }

            return votes;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private string checkWinners(string gameId)
        {//Revisa los ganadores si el juego es mayor

            var round = db.Rounds.Where(s => s.gameId == gameId).ToList();
            var game = db.Games.Where(s => s.gameId == gameId).FirstOrDefault();
            int enemies = 0, citizens = 0;
            foreach (var r in round)
            {
                if(r.result == "enemies")
                {
                    enemies++;
                }else
                {
                    citizens++;
                }
            }
           
            DateTime now = DateTime.Now;
            string fecha = now.ToString("yyyy-MM-dd HH:mm:ss");
  
            if (enemies >= 3)
            {
                game.updatedAt = fecha;
                game.gameStatus = "ended";
                

                db.Games.Update(game);
                db.SaveChanges();

                return "enemies";
               
            }
            else if (citizens >= 3)
            {
                game.updatedAt = fecha;
                game.gameStatus = "ended";


                db.Games.Update(game);
                db.SaveChanges();

                return "citizens";
            }

            return null;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void checkVotes(string gameId, string roundId, List<Player> players)
        {
            var vote = db.VoteRounds.Where(s => s.gameId == gameId && s.roundId == roundId).ToList();

            var round = db.Rounds.Where(s => s.gameId == gameId && s.roundId == roundId).FirstOrDefault();

            var game = db.Games.Where(s => s.gameId == gameId).FirstOrDefault();

            int countFalse = 0;
            int countTrue = 0;

            int persons = players.Count();

            if (persons == vote.Count()) { //revisa que ya todos votaron
                foreach (var v in vote)
                {//revisa cada voto

                    if (v.vote == false)//Revisa que el voto sea falso
                    {
                        countFalse++;
                    }
                    else
                    {
                        countTrue++;
                    }

                    
                }
                
                if (countFalse > (persons / 2))//Si la cantidad de votos negativos es mayor o igual a al mitad todos
                {
                    var group = db.GroupRounds.Where(s => s.gameId == gameId && s.roundId == roundId).ToList();
                    var votes = db.VoteRounds.Where(s => s.gameId == gameId && s.roundId == roundId).ToList();
                    foreach (var g in group)
                    {//Elimino el grupo propuesto
                        db.GroupRounds.Remove(g);
                        
                        
                    }
                    foreach(var v in votes)
                    {
                        db.VoteRounds.Remove(v);
                    }
                        //Un update de la ronda
                    if(round.phase == "vote1")
                    {
                       round.phase = "vote2";
                       round.roundStatus = "waiting-on-leader";
                    
                    }
                    else if(round.phase == "vote2")
                    {
                       round.phase = "vote3";
                       round.roundStatus = "waiting-on-leader";
                    }
                    else
                    {
                        round.result = "enemies";
                        round.roundStatus = "ended";
                        db.Rounds.Update(round);
                        db.SaveChanges();
                        string winners = checkWinners(gameId);
                        if (winners == null)
                        {
                            var roundNext = new Round();
                            Guid roundIdN = Guid.NewGuid();
                            roundNext.roundId = roundIdN.ToString();
                            roundNext.gameId = gameId;
                            roundNext.leader = players[getNum(players.Count)].playerName;
                            roundNext.roundStatus = "waiting-on-leader";
                            roundNext.result = "none";
                            roundNext.phase = "vote1";
                            DateTime now = DateTime.Now;
                            string fecha = now.ToString("yyyy-MM-dd HH:mm:ss");
                            game.updatedAt = fecha;
                            game.currentRound = roundIdN.ToString();


                            db.Games.Update(game);
                            db.Rounds.Add(roundNext);
                        }
                        

                    }

                    db.Rounds.Update(round);
                    db.SaveChanges();

                }
                else if(countTrue >= (persons / 2))
                {
                    
                    round.roundStatus = "waiting-on-group";
                    db.Rounds.Update(round);
                    db.SaveChanges();
                }
            }

        }

        [Route("games/{gameId?}/rounds/{roundId}")]
        [HttpPost]
        public ActionResult voteGroup([FromBody] RequestVote? requestVote, string? gameId, string? roundId, [FromHeader] string? password, [FromHeader] string? player)
        {
            //Pregutar data.result
            //Duda data.status
            //Hecho por Milena
            try
            {
                if (string.IsNullOrEmpty(gameId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The field gameId can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(roundId))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The field round id can not be empty";
                    return StatusCode(400, error);
                }
                if (requestVote == null || requestVote.vote == null)
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The field vote can not be empty";
                    return StatusCode(400, error);
                }
                if (string.IsNullOrEmpty(player))
                {
                    var error = new ErrorResponse();
                    error.status = 400;
                    error.msg = "The field player can not be empty";
                    return StatusCode(400, error);
                }

                var gameFind = db.Games.Find(gameId);

                
                
                if (gameFind == null)
                {
                    var error = new ErrorResponse();
                    error.status = 404;
                    error.msg = "The specified resource was not found";
                    return StatusCode(404, error);
                }
                else
                {
                    var roundValid = db.Rounds.Where(s => s.roundId == roundId).FirstOrDefault();

                    if (roundValid == null)
                    {
                        var error = new ErrorResponse();
                        error.status = 404;
                        error.msg = "The specified resource was not found";
                        return StatusCode(404, error);
                    }

                    if(gameFind.gamePassword == null)
                    {
                        if (!playerG(gameId, player))
                        {
                            var error = new ErrorResponse();
                            error.status = 403;
                            error.msg = "Not part of the group";
                            return StatusCode(403, error);
                        }
                        else
                        {
                            
                            if (gameFind.gameStatus != "rounds" || roundValid.roundStatus!="voting")
                            {
                                var error = new ErrorResponse();
                                error.status = 428;
                                error.msg = "This action is not allowed at this time";
                                return StatusCode(428, error);
                            }
                            else
                            {
                                var playerId = idPlayer(gameId, player);
                                var voteFound = db.VoteRounds.Where(s => s.playerId == playerId && s.gameId == gameId && s.roundId == roundId).ToList();
                                if (voteFound.Count > 0)
                                {
                                    var error = new ErrorResponse();
                                    error.status = 409;
                                    error.msg = "Asset already exists";
                                    return StatusCode(409, error);
                                }
                                else
                                {

                                    var players = db.Players.Where(s => s.gameId == gameId).ToList();

                                    VoteRound voteRound = new VoteRound();
                                    voteRound.gameId = gameId;
                                    voteRound.roundId = roundId;
                                    voteRound.playerId = playerId;
                                    voteRound.vote = requestVote.vote;


                                    db.VoteRounds.Add(voteRound);
                                    db.SaveChanges();

                                    
                                    var phase = roundValid.phase;
                                    var leader = roundValid.leader;
                                    

                                    var data = new DataVote();
                                    data.id = gameId;
                                    data.leader = leader;
                                    data.status = "voting"; //revisar
                                    data.result = "none";//mucho ojo
                                    data.phase = phase;
                                    data.group = getGroups(gameId, roundId);
                                    data.vote = listVotes(gameId, roundId);

                                    checkVotes(gameId, roundId, players);

                                    var response = new ResposeVote();
                                    response.status = 200;
                                    response.msg = "Voted successfully";
                                    response.data = data;
                                    return StatusCode(200, response);

                                }

                            }
                        }
                    }
                    else
                    {
                        if (password == gameFind.gamePassword)
                        {
                            if (!playerG(gameId, player))
                            {
                                var error = new ErrorResponse();
                                error.status = 403;
                                error.msg = "Not part  of the group";
                                return StatusCode(403, error);
                            }
                            else
                            {
                                if (gameFind.gameStatus != "rounds" || roundValid.roundStatus != "voting")
                                {
                                    var error = new ErrorResponse();
                                    error.status = 428;
                                    error.msg = "This action is not allowed at this time";
                                    return StatusCode(428, error);
                                }
                                else
                                {
                                    var playerId = idPlayer(gameId, player);
                                    var voteFound = db.VoteRounds.Where(s => s.playerId == playerId && s.gameId == gameId && s.roundId == roundId).ToList();
                                    if (voteFound.Count > 0)
                                    {
                                        var error = new ErrorResponse();
                                        error.status = 409;
                                        error.msg = "Asset already exists";
                                        return StatusCode(409, error);
                                    }
                                    else
                                    {

                                        var players = db.Players.Where(s => s.gameId == gameId).ToList();

                                        var round = db.Rounds.Find(gameId);


                                        VoteRound voteRound = new VoteRound();
                                        voteRound.gameId = gameId;
                                        voteRound.roundId = roundId;
                                        voteRound.playerId = playerId;
                                        voteRound.vote = requestVote.vote;

                                        db.VoteRounds.Add(voteRound);
                                        db.SaveChanges();

                                        var phase = roundValid.phase;

                                        var leader = roundValid.leader;


                                        var data = new DataVote();
                                        data.id = gameId;
                                        data.leader = leader;
                                        data.status = "voting"; //revisar
                                        data.result = "none";//mucho ojo
                                        data.phase = phase;
                                        data.group = getGroups(gameId, roundId);
                                        data.vote = listVotes(gameId, roundId);

                                        checkVotes(gameId, roundId, players);

                                        var response = new ResposeVote();
                                        response.status = 200;
                                        response.msg = "Voted successfully";
                                        response.data = data;
                                        return StatusCode(200, response);

                                    }

                                }
                            }
                        }
                        else
                        {
                            var error = new ErrorResponse();
                            error.status = 401;
                            error.msg = "Invalid credentials";
                            return StatusCode(401, error);
                        }
                    }


                    
                }

            }
            catch (Exception ex)
            {
                var error = new ErrorResponse();
                error.status = 400;
                error.msg = "Client Error";
                return StatusCode(400, error);
            }
        }


        //METHOD GET GROUPS
        [ApiExplorerSettings(IgnoreApi = true)]
        private void VoteControl(string gameId, string roundId)
        {
            string var = "si";
            int valueVotes = 0;
            int valueTrue = 0;
            int valueFalse = 0;
            ICollection<bool?> vote=listVotes(gameId, roundId);
            int tempPlayers = getPlayers(gameId).Count();
            int mdlPlayers = tempPlayers / 2;
            
            foreach (var vot in vote) {
                if (vot==true)
                {
                    valueTrue++;
                    valueVotes++;
                }
                else
                {
                    valueFalse++;
                    valueVotes++;
                }
            }
            if (tempPlayers == valueVotes)
            {
                if (valueFalse >= mdlPlayers)
                {
                    //Se tiene que eliminar propuse group
                }
                else
                {
                    //se cambia el status
                }
            }
        }
    }
}
