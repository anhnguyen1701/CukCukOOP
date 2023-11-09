using CukCukOOP.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace CukCukOOP.Controllers
{
    public class BaseController<T> : Controller
    {
        protected readonly string _connectionString;
        protected readonly MySqlConnection _conn;
        protected string _tableName;
        protected string _idName;

        public BaseController()
        {
            this._connectionString = "Host=18.179.16.166;" +
                " Port=3306;" +
                " Database = WEB081_MF1790_NLAnh;" +
                " User Id = nvmanh; " +
                "Password = 12345678";
            this._conn = new MySqlConnection(this._connectionString);
            this._tableName = typeof(T).Name;
            this._idName = typeof(T).Name + "Id";
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var sql = $"SELECT * FROM {_tableName}";
                var result = _conn.Query<Employee>(sql);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { devMsg = ex.Message, userMsg = "Lỗi hệ thống", data = "" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var sql = $"select * from {_tableName} where {_idName} = '{id}'";
                var result = _conn.QueryFirstOrDefault<T>(sql);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { devMsg = ex.Message, userMsg = "Lỗi hệ thống", data = "" });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] T entity)
        {
            try
            {
                var cols = "";
                var paramms = "";

                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    cols += $"{prop.Name}, ";
                    paramms += $"@{prop.Name}, ";
                }
                cols = cols.Substring(0, cols.Length - 2);
                paramms = paramms.Substring(0, paramms.Length - 2);

                entity.GetType().GetProperty(_idName)?.SetValue(entity, value: Guid.NewGuid());

                var sql = $"INSERT INTO {_tableName} ({cols}) VALUES ({paramms})";
                var result = _conn.Execute(sql, entity);
                if (result > 0)
                    return Created("", new { devMsg = "", userMsg = "Thêm tành công", data = entity });
                return BadRequest(new { devMsg = "Lỗi", userMsg = "Lỗi", data = entity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { devMsg = ex.Message, userMsg = "Lỗi hệ thống", data = "" });
            }
        }


        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] T entity)
        {
            try
            {
                var paramms = "";

                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    paramms += $"{prop.Name} = @{prop.Name}, ";
                }
                paramms = paramms.Substring(0, paramms.Length - 2);
                entity.GetType().GetProperty(_idName)?.SetValue(entity, value: id);

                var sql = $"UPDATE {_tableName} SET {paramms} WHERE {_idName}= '{id}'";
                var result = _conn.Execute(sql, entity);

                return Ok(new { devMsg = "", userMsg = "Cập nhật thành công", data = entity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { devMsg = ex.Message, userMsg = "Lỗi hệ thống", data = "" });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(Guid id)
        {
            try
            {
                var sql = $"delete from {_tableName} where {_idName} = '{id}'";
                var result = _conn.Execute(sql);

                return Ok(new { devMsg = "", userMsg = "Xóa thành công", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { devMsg = ex.Message, userMsg = "Lỗi hệ thống", data = "" });
            }
        }
    }
}
