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
        Match dbMatch = _context.Matches.FirstOrDefault(m => m.MatchId == MatchId)!;
        if (dbMatch == null) throw new Exception("Match not founded");
        Team findedTeam = _context.Teams.FirstOrDefault(t => t.TeamId == TeamId)!;
        if (findedTeam == null) throw new Exception("Team not founded");
        string newBetValue = BetValue.Replace(",", ".");
        if (dbMatch.MatchTeamAId != TeamId && dbMatch.MatchTeamBId != TeamId) throw new Exception("Team is not in this match");
        if (dbMatch.MatchTeamAId == TeamId) dbMatch.MatchTeamAValue += decimal.Parse(newBetValue, CultureInfo.InvariantCulture);
        else dbMatch.MatchTeamBValue += decimal.Parse(newBetValue, CultureInfo.InvariantCulture);

        _context.Matches.Update(dbMatch);
        _context.SaveChanges();

        return new Match
        {
            MatchId = MatchId,
            MatchDate = dbMatch.MatchDate,
            MatchTeamAId = dbMatch.MatchTeamAId,
            MatchTeamBId = dbMatch.MatchTeamBId,
            MatchTeamAValue = dbMatch.MatchTeamAValue,
            MatchTeamBValue = dbMatch.MatchTeamBValue,
            MatchFinished = dbMatch.MatchFinished,
            MatchWinnerId = dbMatch.MatchWinnerId,
            MatchTeamA = null,
            MatchTeamB = null,
            Bets = null,
        };
    }
}