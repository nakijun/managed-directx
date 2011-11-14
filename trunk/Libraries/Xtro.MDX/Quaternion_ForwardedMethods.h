void Quaternion::Multiply(Quaternion Value)
{
	Quaternion This = Quaternion(this->X, this->Y, this->Z, this->W);
	Quaternion Q;
	Direct3DX10::Functions::QuaternionMultiply(Q, This, Value);

	X = Q.X;
	Y = Q.Y;
	Z = Q.Z;
	W = Q.W;
}

Quaternion Quaternion::operator * (Quaternion Value1, Quaternion Value2)
{
	Quaternion Q;
	Direct3DX10::Functions::QuaternionMultiply(Q, Value1, Value2);

	return Q;
}

