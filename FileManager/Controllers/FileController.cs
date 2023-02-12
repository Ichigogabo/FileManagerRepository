using FileManager.ViewModels.Field;
using FileManagerClassLibrary.Models;
using FileManagerClassLibrary.Services.BlobStorageService;
using FileManagerClassLibrary.Services.CosmoDbService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManager.Controllers
{
    public class FileController : Controller
    {
        private readonly ICosmosDbService _cosmoDbService;
        private readonly IBlobStorageService _blobStorageService;

        public FileController(ICosmosDbService cosmoDbService)
        {
            _cosmoDbService = cosmoDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _cosmoDbService.GetAsync("Select * From C"));
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("UploadFile")]
        public async Task<IActionResult> UploadFile(FileVM fieldVM)
        {
            try
            {
                string blobName = await _blobStorageService.UploadAsync(fieldVM.File);
                Metadata metadata = _cosmoDbService.MapData(fieldVM.File, blobName, fieldVM.Description);
                await _cosmoDbService.AddAsync(metadata);
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Create", fieldVM);
            }
        }
    }
}
