using MissionsService.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace MissionsService.Controllers
{
    public class MissionsController : ODataController
    {
        
        MissionsContext db = new MissionsContext();
        
        //Проверка существования
        private bool MissionExists(int key)
        {
            return db.Missions.Any(p => p.Id == key);
        }

        //Получение всего списка
        [EnableQuery]
        public IQueryable<Mission> Get()
        {
            return db.Missions;
        }

        //Получение сущности по Id
        [EnableQuery]
        public SingleResult<Mission> Get([FromODataUri] int key)
        {
            IQueryable<Mission> result = db.Missions.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        //Создание сущности
        public async Task<IHttpActionResult> Post(Mission mission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Missions.Add(mission);
            await db.SaveChangesAsync();
            return Created(mission);
        }

        //Редактирование отдельных полей сущности
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Mission> mission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Missions.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            mission.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(entity);
        }

        //Перезаписей сущности целиком
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Mission update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            db.Entry(update).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }

        //Удаление
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var mission = await db.Missions.FindAsync(key);
            if (mission == null)
            {
                return NotFound();
            }
            db.Missions.Remove(mission);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


