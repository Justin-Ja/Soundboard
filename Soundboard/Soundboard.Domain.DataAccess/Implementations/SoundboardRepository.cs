using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Soundboard.Domain.DataAccess.Implementations;


public class SoundboardRepository : ISoundboardRepository
{
    private readonly SoundboardDbContext _context;

    public SoundboardRepository(SoundboardDbContext context)
    {
        _context = context;
    }

    public async Task<List<SoundButton>> GetAllSoundButtonsAsync()
    {
        return await _context.SoundButtons
            .Include(sb => sb.GridLayout)
            .ToListAsync();
    }

    public async Task<List<SoundButtonGridLayout>> GetAllButtonGridsAsync()
    {
        return await _context.GridLayouts
            .Include(g => g.SoundButtons)
            .ToListAsync();
    }

    //TODO: SOme of these I added thining Id use them but so far not so. Delete any unused or unlikely to use in the future.
    public async Task<SoundButton> AddSoundButtonAsync(SoundButton soundButton)
    {
        _context.SoundButtons.Add(soundButton);
        await _context.SaveChangesAsync();
        return soundButton;
    }

    public async Task<SoundButtonGridLayout> AddButtonGridAsync(SoundButtonGridLayout gridLayout)
    {
        _context.GridLayouts.Add(gridLayout);
        await _context.SaveChangesAsync();
        return gridLayout;
    }

    public async Task UpdateSoundButtonAsync(SoundButton soundButton)
    {
        _context.SoundButtons.Update(soundButton);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSoundButtonAsync(Guid id)
    {
        var soundButton = await _context.SoundButtons.FindAsync(id);
        if (soundButton != null)
        {
            _context.SoundButtons.Remove(soundButton);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<SoundButton> GetSoundButtonByIdAsync(Guid id)
    {
        return await _context.SoundButtons
            .Include(sb => sb.GridLayout)
            .FirstAsync();
    }

    public async Task<SoundButtonGridLayout> AddButtonGridWithSoundButtonsAsync(SoundButtonGridLayout grid)
    {
        if (grid.Guid == Guid.Empty)
            grid.Guid = Guid.NewGuid();

        foreach (var soundButton in grid.SoundButtons)
        {
            if (soundButton.Guid == Guid.Empty)
                soundButton.Guid = Guid.NewGuid();
            soundButton.GridId = grid.Guid;
        }

        _context.GridLayouts.Add(grid);
        await _context.SaveChangesAsync();

        return await _context.GridLayouts
            .Include(g => g.SoundButtons)
            .FirstAsync(g => g.Guid == grid.Guid);
    }

    public async Task<SoundButtonGridLayout> UpdateButtonGridAsync(SoundButtonGridLayout grid)
    {
        var existingGrid = await _context.GridLayouts
            .Include(g => g.SoundButtons)
            .FirstAsync(g => g.Guid == grid.Guid);

        existingGrid.Name = grid.Name;

        _context.SoundButtons.RemoveRange(existingGrid.SoundButtons);

        foreach (var soundButton in grid.SoundButtons)
        {
            if (soundButton.Guid == Guid.Empty)
                soundButton.Guid = Guid.NewGuid();

            soundButton.GridId = grid.Guid;
            _context.SoundButtons.Add(soundButton);
        }

        await _context.SaveChangesAsync();

        return await _context.GridLayouts
            .Include(g => g.SoundButtons)
            .FirstAsync(g => g.Guid == grid.Guid);
    }
}

