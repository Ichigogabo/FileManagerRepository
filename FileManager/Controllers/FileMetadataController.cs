using FileManagerClassLibrary.Interfaces;
using FileManagerClassLibrary.Models;
using FileManagerClassLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return View(await _unitOfWork.FileMetadata.GetAllFileMetadaAsync());
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("UploadFile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(CreateFileViewModel newFile)
        {
            try
            {                
                var blobResponse =  await _unitOfWork.BlobStorage.UploadAsync(newFile.File);
                FileMetadata metadata = _unitOfWork.FileMetadata.MapData(newFile.File, blobResponse.FileName, newFile.Description);
                
                await _unitOfWork.FileMetadata.AddAsync(metadata);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
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


    }
}
