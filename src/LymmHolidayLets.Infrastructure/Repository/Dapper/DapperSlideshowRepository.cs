using Dapper;
using LymmHolidayLets.Domain.Model.Slideshow.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperSlideshowRepository(DbSession session)
        : RepositoryBase<Slideshow>(session), ISlideshowRepository
    {
        public IEnumerable<Slideshow> GetAll()
        {
            const string procedure = "Slideshow_GetAll";

            try
            {
                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                    commandType: CommandType.StoredProcedure);


                return results.Select(slideshow => new Slideshow(slideshow.ID, slideshow.ImagePath, slideshow.ImagePathAlt, slideshow.CaptionTitle, slideshow.Caption, slideshow.ShortMobileCaption, slideshow.Link, slideshow.SequenceOrder, slideshow.Visible)).ToList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all slideshows with the procedure {procedure}", ex);
            }
        }

        public Slideshow? GetById(byte id)
        {
            const string procedure = "Slideshow_GetById";

            try
            {
                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var slideShow = new Slideshow(result.ID,
                    result.ImagePath, result.ImagePathAlt, result.CaptionTitle, result.Caption, result.ShortMobileCaption,
                    result.Link, result.SequenceOrder, result.Visible);
                

                return slideShow;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a slideShow with the procedure {procedure}", ex);
            }
        }

        public void Create(Slideshow slideshow)
        {
            const string procedure = "Slideshow_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    slideshow.ImagePath,
                    slideshow.ImagePathAlt,
                    slideshow.CaptionTitle,
                    slideshow.Caption,
                    slideshow.ShortMobileCaption,
                    slideshow.Link,
                    slideshow.SequenceOrder,
                    slideshow.Visible
                },
                commandType: CommandType.StoredProcedure);                
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating a slideshow with the procedure {procedure}", ex);
            }
        }

        public void Update(Slideshow slideshow)
        {
            const string procedure = "Slideshow_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure,
                new
                {
                    slideshow.ID,
                    slideshow.ImagePath,
                    slideshow.ImagePathAlt,
                    slideshow.CaptionTitle,
                    slideshow.Caption,
                    slideshow.ShortMobileCaption,
                    slideshow.Link,
                    slideshow.SequenceOrder,
                    slideshow.Visible
                },
                commandType: CommandType.StoredProcedure);
               
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating a slideshow with the procedure {procedure}", ex);
            }
        }

        public void Delete(byte id)
        {
            const string procedure = "Slideshow_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    id
                }, commandType: CommandType.StoredProcedure);                
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a slideshow with the procedure {procedure}", ex);
            }
        }
    }
}
