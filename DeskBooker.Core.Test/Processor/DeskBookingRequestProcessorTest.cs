using DeskBooker.Core.Domain;
using DeskBooker.Core.DataInterface;
using Moq;
using DeskBooker.Core.Domian;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessorTest
{
    private readonly DeskBookingRequest _request;
    private readonly DeskBookingRequestProcessor _processor;
    private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
    private readonly List<Desk> _availableDesks;
    private Mock<IDeskRepository> _deskRepositoryMock;

    public DeskBookingRequestProcessorTest()
    {
        _request = new DeskBookingRequest
        {
            FirstName = "Edward",
            LastName = "Aleonope",
            Email = "eddytonia@cardiff.co.uk",
            Date = DateTime.Now,
        };

        _availableDesks = new List<Desk> { new Desk {Id = 7 } };
        _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
       
        _deskRepositoryMock = new Mock<IDeskRepository>();
        _deskRepositoryMock.Setup(_x => _x.GetAvailableDesks(_request.Date))
            .Returns(_availableDesks);
        _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object) ;
       

    }

    [Fact]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
        //arrange
        
       
        //Act
        DeskBookingResult result = _processor.BookDesk(_request);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(_request.FirstName, result.FirstName);
        Assert.Equal(_request.LastName, result.LastName);
        Assert.Equal(_request.Email, result.Email);
        Assert.Equal(_request.Date, result.Date);
    }


    [Fact]
    public void ShouldThrowExceptionIfRequestIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));
        Assert.Equal("request", exception.ParamName);

    }

    [Fact]
    public void ShouldSaveDeskBooking()
    {
        //Arrange
        DeskBooking? saveDeskBooking = null;
        _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
            .Callback<DeskBooking>(deskBooking =>
            {
                saveDeskBooking = deskBooking;
            });

        _processor.BookDesk(_request);

        _deskBookingRepositoryMock.Verify(x=>x.Save(It.IsAny<DeskBooking>()), Times.Once);

        Assert.NotNull(saveDeskBooking);
        Assert.Equal(_request.FirstName, saveDeskBooking.FirstName);
        Assert.Equal(_request.LastName, saveDeskBooking.LastName);
        Assert.Equal(_request.Email, saveDeskBooking.Email);
        Assert.Equal(_request.Date, saveDeskBooking.Date);
        Assert.Equal(_availableDesks.First().Id, saveDeskBooking.DeskId);


    }
    [Fact]
    public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
    {
        _availableDesks.Clear();
        _processor.BookDesk(_request);

        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
    }
    [Theory]
    [InlineData(DeskBookingResultCode.Success, true)]
    [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]
    public void ShouldReturnExpectedResultCode(DeskBookingResultCode expectedResultCode, bool isAvailable)
    {
        if(!isAvailable)
        {
            _availableDesks.Clear();
        }
        var result = _processor.BookDesk(_request);

        Assert.Equal(expectedResultCode, result.Code);

    }
}
