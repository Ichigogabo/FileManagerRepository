using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileManager.ApplicationCore.Interfaces;
using FileManager.ApplicationCore.Models;
using FileManager.ApplicationCore.ViewModels;
using System.Security.Claims;

namespace FileManager.Controllers
{
    public class FileMetadataController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FileMetadataController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name;
            return View(await _unitOfWork.FileMetadata.GetAllFileMetadaAsync(userName));
        }

        [HttpGet]
        [ActionName("Details")]
        public async Task<IActionResult> Details(string id)
        {
            return View(await _unitOfWork.FileMetadata.GetByIdAsync(id));
        }

        [HttpGet]
        [ActionName("Create")]
        public IActionResult Create()
        {
            CreateFileViewModel newFile = new CreateFileViewModel();
            return View(newFile);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateFileViewModel newFile)
        {
            try
            {
                #region Validation
                if (newFile.File == null)
                {
                    ModelState.AddModelError("File", "File is Requerid");
                }

                if (string.IsNullOrEmpty(newFile.Description))
                {
                    ModelState.AddModelError("Description", "Description is Requerid");
                }
                #endregion

                if (ModelState.IsValid)
                {
                    var blobResponse = await _unitOfWork.BlobStorage.UploadAsync(newFile.File);
                    if (!blobResponse.Success) return StatusCode(StatusCodes.Status500InternalServerError);

                    var userName = User.Identity.Name;
                    FileMetadata metadata = _unitOfWork.FileMetadata.MapData(newFile.File, userName, blobResponse.FileName, newFile.Description);
                    await _unitOfWork.FileMetadata.AddAsync(metadata);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", newFile);
                }
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Unexpected System Error");
                return View("Create", newFile);
            }
        }

        [HttpGet("DownloadFile")]        
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) return BadRequest();

                var file = await _unitOfWork.BlobStorage.DownloadAsync(fileName);
                return File(file.Content,file.ContentType, file.Name);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            return View(await _unitOfWork.FileMetadata.GetByIdAsync(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, string fileNameInBlobStorage)
        {

            if (await _unitOfWork.BlobStorage.DeleteAsync(fileNameInBlobStorage))
            {
                await _unitOfWork.FileMetadata.DeleteAsync(id);
                return RedirectToAction("Index");
            }            
            
            return View();
        }
    }
}
