using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace WindowsFormsApp
{
    public class DatabaseHelper
    {
        private string connectionString;
        private const string DB_NAME = "food_database.db";
        private string databaseFilePath;
        private string imagesFolderPath;

        public string DatabaseFilePath => databaseFilePath;
        public string ImagesFolderPath => imagesFolderPath;

        public DatabaseHelper()
        {
            // Tạo database trong thư mục hiện tại (giống Bai4)
            databaseFilePath = DB_NAME;
            connectionString = $"Data Source={DB_NAME}";
            
            // Tạo thư mục Images nếu chưa có
            imagesFolderPath = "Images";
            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Tạo bảng NguoiDung (Người đóng góp)
                var createNguoiDungTable = @"
                    CREATE TABLE IF NOT EXISTS NguoiDung (
                        IDNCC INTEGER PRIMARY KEY AUTOINCREMENT,
                        HoVaTen TEXT NOT NULL,
                        QuyenHan TEXT NOT NULL
                    )";

                // Tạo bảng MonAn (Món ăn)
                var createMonAnTable = @"
                    CREATE TABLE IF NOT EXISTS MonAn (
                        IDMA INTEGER PRIMARY KEY AUTOINCREMENT,
                        TenMonAn TEXT NOT NULL,
                        HinhAnh TEXT,
                        IDNCC INTEGER NOT NULL,
                        FOREIGN KEY (IDNCC) REFERENCES NguoiDung(IDNCC)
                    )";

                var command = connection.CreateCommand();
                command.CommandText = createNguoiDungTable;
                command.ExecuteNonQuery();

                command.CommandText = createMonAnTable;
                command.ExecuteNonQuery();
            }
        }

        public bool AddNguoiDung(string hoVaTen, string quyenHan)
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO NguoiDung (HoVaTen, QuyenHan) 
                        VALUES (@hoVaTen, @quyenHan)";
                    command.Parameters.AddWithValue("@hoVaTen", hoVaTen);
                    command.Parameters.AddWithValue("@quyenHan", quyenHan);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddMonAn(string tenMonAn, string hinhAnh, int idNCC)
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO MonAn (TenMonAn, HinhAnh, IDNCC) 
                        VALUES (@tenMonAn, @hinhAnh, @idNCC)";
                    command.Parameters.AddWithValue("@tenMonAn", tenMonAn);
                    command.Parameters.AddWithValue("@hinhAnh", string.IsNullOrEmpty(hinhAnh) ? DBNull.Value : hinhAnh);
                    command.Parameters.AddWithValue("@idNCC", idNCC);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<MonAnInfo> GetAllMonAn()
        {
            var monAnList = new List<MonAnInfo>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT m.IDMA, m.TenMonAn, m.HinhAnh, m.IDNCC, n.HoVaTen, n.QuyenHan
                    FROM MonAn m
                    INNER JOIN NguoiDung n ON m.IDNCC = n.IDNCC
                    ORDER BY m.IDMA";
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        monAnList.Add(new MonAnInfo
                        {
                            IDMA = reader.GetInt32(0),
                            TenMonAn = reader.GetString(1),
                            HinhAnh = reader.IsDBNull(2) ? null : reader.GetString(2),
                            IDNCC = reader.GetInt32(3),
                            HoVaTen = reader.GetString(4),
                            QuyenHan = reader.GetString(5)
                        });
                    }
                }
            }
            return monAnList;
        }

        public List<MonAnInfo> GetMonAnByUser(int idNCC)
        {
            var monAnList = new List<MonAnInfo>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT m.IDMA, m.TenMonAn, m.HinhAnh, m.IDNCC, n.HoVaTen, n.QuyenHan
                    FROM MonAn m
                    INNER JOIN NguoiDung n ON m.IDNCC = n.IDNCC
                    WHERE m.IDNCC = @idNCC
                    ORDER BY m.IDMA";
                command.Parameters.AddWithValue("@idNCC", idNCC);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        monAnList.Add(new MonAnInfo
                        {
                            IDMA = reader.GetInt32(0),
                            TenMonAn = reader.GetString(1),
                            HinhAnh = reader.IsDBNull(2) ? null : reader.GetString(2),
                            IDNCC = reader.GetInt32(3),
                            HoVaTen = reader.GetString(4),
                            QuyenHan = reader.GetString(5)
                        });
                    }
                }
            }
            return monAnList;
        }

        public MonAnInfo GetRandomMonAn()
        {
            var allMonAn = GetAllMonAn();
            if (allMonAn.Count == 0) return null;
            
            Random random = new Random();
            return allMonAn[random.Next(allMonAn.Count)];
        }

        public MonAnInfo GetRandomMonAnByUser(int idNCC)
        {
            var monAnList = GetMonAnByUser(idNCC);
            if (monAnList.Count == 0) return null;
            
            Random random = new Random();
            return monAnList[random.Next(monAnList.Count)];
        }

        public List<NguoiDungInfo> GetAllNguoiDung()
        {
            var nguoiDungList = new List<NguoiDungInfo>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT IDNCC, HoVaTen, QuyenHan FROM NguoiDung ORDER BY IDNCC";
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nguoiDungList.Add(new NguoiDungInfo
                        {
                            IDNCC = reader.GetInt32(0),
                            HoVaTen = reader.GetString(1),
                            QuyenHan = reader.GetString(2)
                        });
                    }
                }
            }
            return nguoiDungList;
        }

        public void AddSampleData()
        {
            // Thêm người dùng mẫu
            if (GetAllNguoiDung().Count == 0)
            {
                AddNguoiDung("Nguyễn Văn A", "Admin");
                AddNguoiDung("Trần Thị B", "User");
                AddNguoiDung("Lê Văn C", "User");
            }

            var nguoiDungList = GetAllNguoiDung();
            if (nguoiDungList.Count == 0) return;

            // Thêm món ăn mẫu
            var monAnMau = new List<(string ten, string hinh, int idNCC)>
            {
                ("Phở Bò", "pho_bo.jpg", nguoiDungList[0].IDNCC),
                ("Bánh Mì", "banh_mi.jpg", nguoiDungList[0].IDNCC),
                ("Bún Bò Huế", "bun_bo_hue.jpg", nguoiDungList[1].IDNCC),
                ("Cơm Tấm", "com_tam.jpg", nguoiDungList[1].IDNCC),
                ("Bánh Xèo", "banh_xeo.jpg", nguoiDungList[2].IDNCC),
                ("Cháo Lòng", "chao_long.jpg", nguoiDungList[2].IDNCC)
            };

            var existingMonAn = GetAllMonAn();
            foreach (var mon in monAnMau)
            {
                if (!existingMonAn.Any(m => m.TenMonAn.Equals(mon.ten, StringComparison.OrdinalIgnoreCase)))
                {
                    AddMonAn(mon.ten, mon.hinh, mon.idNCC);
                }
            }
        }
    }
}

