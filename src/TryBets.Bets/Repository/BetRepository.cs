using TryBets.Bets.DTO;
using TryBets.Bets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TryBets.Bets.Repository;

public class BetRepository : IBetRepository
{
    protected readonly ITryBetsContext _context;
    public BetRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public BetDTOResponse Post(BetDTORequest betRequest, string email)
    {
        User userDb = _context.Users.FirstOrDefault(x => x.Email == email)!;
        if (userDb == null) throw new Exception("User not founded");

        Match matchDb = _context.Matches.FirstOrDefault(m => m.MatchId == betRequest.MatchId)!;
        if (matchDb == null) throw new Exception("Match not founded");

        Team findedTeam = _context.Teams.FirstOrDefault(t => t.TeamId == betRequest.TeamId)!;
        if (findedTeam == null) throw new Exception("Team not founded");

        if (matchDb.MatchFinished) throw new Exception("Match finished");

        if (matchDb.MatchTeamAId != betRequest.TeamId && matchDb.MatchTeamBId != betRequest.TeamId) throw new Exception("Team is not in this match");

        Bet newBet = new Bet
        {
            UserId = userDb.UserId,
            MatchId = betRequest.MatchId,
            TeamId = betRequest.TeamId,
            BetValue = betRequest.BetValue
        };
        _context.Bets.Add(newBet);
        _context.SaveChanges();

        Bet createdBet = _context.Bets.Include(b => b.Team).Include(b => b.Match).Where(b => b.BetId == newBet.BetId).FirstOrDefault()!;

        return new BetDTOResponse
        {
            BetId = createdBet.BetId,
            MatchId = createdBet.MatchId,
            TeamId = createdBet.TeamId,
            BetValue = createdBet.BetValue,
            MatchDate = createdBet.Match!.MatchDate,
            TeamName = createdBet.Team!.TeamName,
            Email = createdBet.User!.Email
        };
    }
    public BetDTOResponse Get(int BetId, string email)
    {
        User userDb = _context.Users.FirstOrDefault(x => x.Email == email)!;
        if (userDb == null) throw new Exception("User not founded");

        Bet betDb = _context.Bets.Include(b => b.Team).Include(b => b.Match).Where(b => b.BetId == BetId).FirstOrDefault()!;
        if (betDb == null) throw new Exception("Bet not founded");

        if (betDb.User!.Email != email) throw new Exception("Bet view not allowed");

        return new BetDTOResponse
        {
            BetId = betDb.BetId,
            MatchId = betDb.MatchId,
            TeamId = betDb.TeamId,
            BetValue = betDb.BetValue,
            MatchDate = betDb.Match!.MatchDate,
            TeamName = betDb.Team!.TeamName,
            Email = betDb.User!.Email
        };
    }
}