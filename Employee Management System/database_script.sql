USE [master]
GO
/****** Object:  Database [Management_system]    Script Date: 3/29/2023 10:37:44 PM ******/
CREATE DATABASE [Management_system]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Management_system', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Management_system.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Management_system_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Management_system_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Management_system] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Management_system].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Management_system] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Management_system] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Management_system] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Management_system] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Management_system] SET ARITHABORT OFF 
GO
ALTER DATABASE [Management_system] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Management_system] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Management_system] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Management_system] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Management_system] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Management_system] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Management_system] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Management_system] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Management_system] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Management_system] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Management_system] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Management_system] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Management_system] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Management_system] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Management_system] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Management_system] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Management_system] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Management_system] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Management_system] SET  MULTI_USER 
GO
ALTER DATABASE [Management_system] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Management_system] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Management_system] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Management_system] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Management_system] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Management_system] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Management_system] SET QUERY_STORE = OFF
GO
USE [Management_system]
GO
/****** Object:  Table [dbo].[tbl_employee]    Script Date: 3/29/2023 10:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_employee](
	[EMP_DocNo] [nvarchar](50) NULL,
	[EMP_DocType] [nvarchar](50) NULL,
	[EMP_Name] [nvarchar](50) NULL,
	[EMP_Address] [nvarchar](max) NULL,
	[EMP_DOB] [nvarchar](50) NULL,
	[EMP_Gender] [nvarchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[USP_Addemp]    Script Date: 3/29/2023 10:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[USP_Addemp]
	-- Add the parameters for the stored procedure here
	@name		nvarchar(50),
	@address	nvarchar(max),
	@dob    	nvarchar(50),
	@gender 	nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
DECLARE 
		@NewDocNo		nvarchar(20)
		SELECT  @NewDocNo=IIF(COUNT(*)=0,1000,MAX([EMP_DocNo])+1) FROM [dbo].[tbl_employee]

		INSERT INTO [dbo].[tbl_employee]
           ([EMP_DocNo]
           ,[EMP_DocType]
           ,[EMP_Name]
           ,[EMP_Address]
           ,[EMP_DOB]
           ,[EMP_Gender])
     VALUES
           (
		  @NewDocNo		, 
		  'EMP'			,
		  @name			,
		  @address 		,
		  @dob     		,
		  @gender 

		  )

if @@ROWCOUNT>0
begin
select 'Successfully added employee' as message
end 
else
begin
select 'Try again' as message

end
END
GO
/****** Object:  StoredProcedure [dbo].[USP_Deleteemp]    Script Date: 3/29/2023 10:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USP_Deleteemp]
	-- Add the parameters for the stored procedure here
	
	@EMP_DocNo 			nvarchar(50)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    -- Insert statements for procedure here
	
DELETE FROM [dbo].[tbl_employee]
      WHERE [EMP_DocNo]=@EMP_DocNo


END 
GO
/****** Object:  StoredProcedure [dbo].[USP_Editemp]    Script Date: 3/29/2023 10:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[USP_Editemp]
	-- Add the parameters for the stored procedure here
	@name			nvarchar(50)    ,
	@address		nvarchar(max)	,
	@dob			nvarchar(50)	,
	@gender 		nvarchar(50)	,
	@docno 			nvarchar(50)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    -- Insert statements for procedure here
	
UPDATE [dbo].[tbl_employee]
   SET
      [EMP_Name] =   @name				,
      [EMP_Address] = @address			,
      [EMP_DOB] =    @dob				,
      [EMP_Gender] = @gender			
 WHERE  [EMP_DocNo]=@docno

 if @@ROWCOUNT>0
 begin
 select 'Successfully updated' as message
 end
 else
 begin
  select 'Try again' as message

 end


END 
GO
/****** Object:  StoredProcedure [dbo].[USP_EMP_exist]    Script Date: 3/29/2023 10:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[USP_EMP_exist]
	-- Add the parameters for the stored procedure here
	@name		nvarchar(50)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    -- Insert statements for procedure here

select count(*)  as count 
from [dbo].[tbl_employee] 
where [EMP_Name]=@name
END 
GO
/****** Object:  StoredProcedure [dbo].[USP_GetEmpEdit]    Script Date: 3/29/2023 10:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USP_GetEmpEdit]
	-- Add the parameters for the stored procedure here
	@EMP_DocNo    nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    -- Insert statements for procedure here
	
SELECT [EMP_DocNo]
      ,[EMP_DocType]
      ,[EMP_Name]
      ,[EMP_Address]
      ,[EMP_DOB]
      ,[EMP_Gender]
  FROM [dbo].[tbl_employee]   where [EMP_DocNo]=@EMP_DocNo

END 
GO
/****** Object:  StoredProcedure [dbo].[USP_ViewEmp]    Script Date: 3/29/2023 10:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USP_ViewEmp]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    -- Insert statements for procedure here
	
SELECT [EMP_DocNo]
      ,[EMP_DocType]
      ,[EMP_Name]
      ,[EMP_Address]
      ,[EMP_DOB]
      ,[EMP_Gender]
  FROM [dbo].[tbl_employee]

END 
GO
USE [master]
GO
ALTER DATABASE [Management_system] SET  READ_WRITE 
GO
