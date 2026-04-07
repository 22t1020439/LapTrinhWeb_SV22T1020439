1 tạo solution có tên Mysolution
bố sung vào soluton các project sau
solution.HRM project dạng asp.net cort MVC
-solution.HRM
-solution.DomainModel
-solution.Datalayers
-solution.BusinessLayers									
Mã nguồn tổ chức sử dụng kiến trúc mô hinh 3 layer
3. Dự án phần mềm quản lý nhân sự được tổ chức theo kiến trúc
	3-layer bao gồm các thành phần project sau:
-solution.HRM
-solution.DomainModel
-solution.Datalayers
-solution.BusinessLayers
Viết lớp Employee trong Solution.DomainModels sử dụng để biểu diễn dữ liệu cho bảng Employee
4.Viết lớp EmloyeeRepository trong solution.Datalayers cung cấp 
các chức năng xử lý dữ liệu trên emloyee. Yêu cầu
-lớp có contructor có một hàm tham số là chuỗi ConnectionString
-lớp có các hàm sau:
	listAsync: truy vấn danh sách nhân viên
	getAsync : lấy thông itn 1 nhân viên dựa vào id
	addAsync: Bổ sung một nhân viên
	updateAsync cập nhật một nhân viên(không cập nhật id)
	deleteAsync: xóa một nhân viên dựa vào id
-sử dụng dapper
5.viết lớp HRMDataService trong solution.businessLayer cung cấp các chức năng xử lý nghiệp vụ liên quan đến nhân sự
-lớp này là lớp static
-lớp có hàm Initalize có tham số là chuỗi connectionString có chức năng khởi tạo các đối tượng Repository sử dụng trong lớp
-lớp có các hàm sau:
ListEmployeeAsync
GetEmployeeAsync
AddEmployeeAsync
UpdateEmployeeAsync
DeleteEmployeeAsync
6.hiển thị danh sách nhân viên
-khai báo chuỗi tham số kết nối đến csdl
-bổ sung controller có tên là EmployeeController


+tạo solution và các project 
Tạo blank solution có tên SV<Masv>
bố sung cho các solution các project sau:
-<solutionName>.Shop : project dạng ASP.NET core MVC
-<solutionName>.Admin: project dạng ASP.Net core MVC (vd: SV22T1020439.Admin)
-<solutionName>.Datalayers: project dạng class libary
-<solutionName>.BusinessLayers: project dạng class library

thiết kế layout cho admin
thêm AdminLTE4, bootstrap
-mở file layout khách hàng


+các controller và action dự kiến
Home
	Home/Index
Account
	Account/Login
	Account/Logout
	Account/ChangePassword
Supplier
	Supplier/Index
		/Create
		/Edit/{id}
		/Delete/{id}
Customer
	/Index
Hiển thị danh sách khách hàng dưới dạng phân trang
Tìm kiếm khách hàng theo tên
Điểu hướng qua các chức năng khác liên quan tới khách hàng
/Edit/{id}
/ChangePassword/{id}
Shipper
	/Index
	/Create
	/Edit/{id}
	/Delete/{id}
Employee
	/Index
	/Create
	/Edit/{id}
	/Delete/{id}
	/ChangePassword/{id}
	/ChangeRole/{id}
Category
	/Index
	/Create
	/Edit{id}
	/Delete{id}
Product
	
	/Index
	/Create
	/Edit{id}
	/Delete{id}
	/Detail{id}
	/ListAttributes/{id}
	/CreateAddtribute/{id}
	/EditAttribute/{id}?attributeId={attributeId}
	/DeleteAttribute/{id}?attributeId={attributeId}
	/LisPhotos/{id}
	/CreatePhoto/{id}
	/EditPhoto/{id}?PhotoId={photoId}
	/DeletePhoto/{id}?PhotoId={photoId}
	
Order

	/Index
	/Create
	/Edit{id}
	/Delete{id}
		
