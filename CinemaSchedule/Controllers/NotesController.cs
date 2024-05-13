using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using CinemaSchedule.Models;
using CinemaSchedule.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CinemaSchedule.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly DBContext dbContext;
        private readonly UserManager<User> userManager;
        public NotesController(DBContext dBContext, UserManager<User> userManager)
        {
            this.dbContext = dBContext;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult AddNote()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> AddNote(AddNoteViewModel viewModel)
        {

            if (viewModel.NoteName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.NoteName), "Название заметки должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if (viewModel.NoteDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.NoteDescription), "Описание заметки должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }
            var note = new Note()
            {
                NoteName = viewModel.NoteName,
                NoteDescription = viewModel.NoteDescription,
                LastUpdated = DateTime.Now,
                UserId = userManager.GetUserId(User),
            };
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Notes", "Notes");
        }

        [HttpGet]
        [Route("Notes")]
        public async Task<IActionResult> Notes()
        {
            var notes = await dbContext.Notes.Where(x => x.UserId == userManager.GetUserId(User)).ToListAsync();
            notes = notes.OrderByDescending(x => x.LastUpdated).ToList();
            return View(notes);
        }

        [HttpGet]
        public async Task<IActionResult> EditNote(Guid id)
        {

            var note = await dbContext.Notes.FindAsync(id);
            if(note.UserId != userManager.GetUserId(User))
            {
                return RedirectToAction("Notes", "Notes");
            }
            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> EditNote(Note viewModel)
        {
            var note = await dbContext.Notes.FindAsync(viewModel.Id);
            if (note.UserId != userManager.GetUserId(User))
            {
                return RedirectToAction("Notes", "Notes");
            }
            if (viewModel.NoteName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.NoteName), "Название заметки должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if (viewModel.NoteDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.NoteDescription), "Описание заметки должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if(note is not null)
            {
                note.NoteName = viewModel.NoteName;
                note.NoteDescription = viewModel.NoteDescription;
                note.LastUpdated = DateTime.Now;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Notes", "Notes");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Note viewModel)
        {
            
            var note = await dbContext.Notes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

            if(note is not null)
            {
                if (note.UserId != userManager.GetUserId(User))
                {
                    return RedirectToAction("Notes", "Notes");
                }
                dbContext.Notes.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Notes", "Notes");
        }
    }


}
