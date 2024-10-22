namespace dvld_api.models
{
    public static class clsUploadPersonPhoto
    {
        

        public async static Task<string> Upload(IFormFile Photo)
        {
            string DirectoryPath = @"E:\programming_main\MyProjects\images_folder\dvld_images";

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            var fileinfo = new FileInfo(Photo.FileName);

            Guid guid = Guid.NewGuid();

            var PhotoPath = Path.Combine(DirectoryPath, guid + fileinfo.Extension);

            using (var stream = new FileStream(PhotoPath, FileMode.Create))
            {
               await Photo.CopyToAsync(stream);
            }

            return PhotoPath;
        }
    }
}
