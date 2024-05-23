# Website Kinh Doanh Dịch Vụ Gia Sư
## Member
| Full Name | Email |
|-------------|------------|
| [Nguyễn Đức Chí Danh](https://github.com/Dan3105/)  | `chidanh0502@gmail.com` |
| [Hồ Đức Hoàng (Wibu)](https://github.com/Kiritokun0909) | `n20dccn018@student.ptithcm.edu.vn` |
| [Lương Thúy Vy](https://github.com/LuongThuyVy)  | `n20dccn085@student.ptithcm.edu.vn` |

## Description
This project involves the development of a comprehensive online platform for tutoring services. It facilitates the connection between students seeking tutoring and qualified tutors

![](src/home_screen.png)

- **Development Tools**: Visual Studio Code 
- **Programming Languages**: C#, Javascript, HTML, CSS
- **Frameworks**: 
	- .NET (7.0) 
	- Bootstrap 5.0
- **Database**: PostgreSQL
## Installation
### Docker-Compose Setup 
If you prefer to use docker-compose to run this project, follow these steps: 
1. Install [Docker: Accelerated Container Application Development](https://www.docker.com/) on your system. 
2. Clone this repository to your local machine.
	`> git clone https://github.com/Dan3105/TMDT_GiaSuService.git`
3. Navigate to the project directory
	 > in .env file, update the following variables according to your environment
**Note**:  Ensure that the values for `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `POSTGRES_DB` are set to match your PostgreSQL database configuration.
4. Navigate to `GiaSuService/Services/UploadFileService.cs`
- Since project has used Firebase to saving image like avatar, identity image (front side of the card and back side of the card), so we have to config variables below to use Firebase services:
![Happy-Kitten-Meme-05 - Great River Rescue](src/config.png)

5. Navigate to the project directory and run docker-compose.yml
	```markdown 
	> cd %your_directory%  
	> docker-compose up -d
	```
	* **Note**: Building Docker images may take around 3-5 minutes
6. Access the application by opening your web browser and navigating to `http://localhost:5000`

## Features
0. Common Features:
    - Login
    ![](src/login.png)
    - Register
    ![](src/register_customer.png) 
    ![](src/register_tutorp1.png)
    - View List of Tutors
    ![](src/list_tutor.png)
    - View List of Tutor Requests
    ![](src/list_request.png)
1. Admin Features:
	- Create Employee
    ![](src/admin_create_employee.png)
	- Manage Catalog (Session, Subject, Grade)
    ![](src/admin_manage_catalog.png)
	- View list of Invoice
    ![](src/admin_manage_invoice.png)
	- Dashboard
    ![](src/admin_dashboard_invoice.png)
2. Employee Features:
	- Manage Account of Tutors
    ![](src/employee_view_tutorprofile.png)
	- Review Profile Tutor Applications
    ![](src/employee_view_tutorprofile_form.png)
	- Review Tutor Applications with Changed Profiles
    ![](src/employee_review_changedprofile_detailp1.png)
	- Review Tutor Requests
    ![](src/employee_review_tutor_request_detail.png)
	- Manage Tutor Apply Applications in Tutor Request
    ![](src/employee_manage_tutor_apply_applications.png)
	- Update Payment
    ![](src/employee_update_payment.png)
3. Customer Features:
    - Create Tutor Requests
    ![](src/customer_create_tutor_request.png)
    - View Customer's Tutor Requests 
    ![](src/customer_view_their_list_request.png)
4. Tutor Features:
	- Apply to Tutor Request
    ![](src/tutor_apply_request.png)
	- View List of Tutor's applied Tutor Request
    ![](src/tutor_view_their_list_apply.png)
	- Request Changed Profiles 
    ![](src/tutor_request_change_profile.png)
	- View List of Tutor's Invoice
    ![](src/tutor_view_list_invoice.png)

## References
- <a href="https://docs.google.com/document/d/1X0N2yA6C9T4QJeDXyyNrnt2laOWc0hum/edit?usp=sharing&ouid=101107901105800243767&rtpof=true&sd=true">Report Project </a>