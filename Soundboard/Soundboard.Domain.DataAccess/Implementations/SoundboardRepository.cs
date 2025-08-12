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

    public async Task<List<SoundButtonGridLayout>> GetAllCategoriesAsync()
    {
        return await _context.GridLayouts
            .Include(c => c.SoundButtons)
            .ToListAsync();
    }

    public async Task<SoundButton> AddSoundButtonAsync(SoundButton soundButton)
    {
        _context.SoundButtons.Add(soundButton);
        await _context.SaveChangesAsync();
        return soundButton;
    }

    public async Task<SoundButtonGridLayout> AddCategoryAsync(SoundButtonGridLayout gridLayout)
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
}

