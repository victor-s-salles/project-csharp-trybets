using TryBets.Odds.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace TryBets.Odds.Repository;

public class OddRepository : IOddRepository
{
    protected readonly ITryBetsContext _context;
    public OddRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public Match Patch(int MatchId, int TeamId, string BetValue)
    {
        Match matchDb = _context.Matches.FirstOrDefault(m => m.MatchId == MatchId)!;

        if (matchDb == null) throw new Exception("Match not founded");

        Team findedTeam = _context.Teams.FirstOrDefault(t => t.TeamId == TeamId)!;
        if (findedTeam == null) throw new Exception("Team not founded");

        if (matchDb.MatchFinished) throw new Exception("Match finished");

        if (matchDb.MatchTeamAId != TeamId && matchDb.MatchTeamBId != TeamId) throw new Exception("Team is not in this match");

        if (matchDb.MatchTeamAId == TeamId) matchDb.MatchTeamAValue += decimal.Parse(BetValue);
        else matchDb.MatchTeamBValue += decimal.Parse(BetValue);

        _context.Matches.Update(matchDb);
        _context.SaveChanges();

        return matchDb;
    }
}