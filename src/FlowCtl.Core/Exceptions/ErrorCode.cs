namespace FlowCtl.Core.Exceptions;

public enum ErrorCode
{
    None                                            = 0,

    #region Application error codes
    ApplicationStartArgumentIsRequired              = 1001,
    #endregion

    #region Serialization error codes
    Serialization                                   = 1101,
    #endregion

    #region Unknown error codes
    UnknownError                                    = 9999
    #endregion
}