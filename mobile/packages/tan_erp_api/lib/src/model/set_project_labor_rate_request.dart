//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'set_project_labor_rate_request.g.dart';

/// SetProjectLaborRateRequest
///
/// Properties:
/// * [workerUserId] 
/// * [hourlyRate] 
@BuiltValue()
abstract class SetProjectLaborRateRequest implements Built<SetProjectLaborRateRequest, SetProjectLaborRateRequestBuilder> {
  @BuiltValueField(wireName: r'workerUserId')
  String? get workerUserId;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  SetProjectLaborRateRequest._();

  factory SetProjectLaborRateRequest([void updates(SetProjectLaborRateRequestBuilder b)]) = _$SetProjectLaborRateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SetProjectLaborRateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SetProjectLaborRateRequest> get serializer => _$SetProjectLaborRateRequestSerializer();
}

class _$SetProjectLaborRateRequestSerializer implements PrimitiveSerializer<SetProjectLaborRateRequest> {
  @override
  final Iterable<Type> types = const [SetProjectLaborRateRequest, _$SetProjectLaborRateRequest];

  @override
  final String wireName = r'SetProjectLaborRateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SetProjectLaborRateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.workerUserId != null) {
      yield r'workerUserId';
      yield serializers.serialize(
        object.workerUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.hourlyRate != null) {
      yield r'hourlyRate';
      yield serializers.serialize(
        object.hourlyRate,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SetProjectLaborRateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SetProjectLaborRateRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'workerUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workerUserId = valueDes;
          break;
        case r'hourlyRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.hourlyRate = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SetProjectLaborRateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SetProjectLaborRateRequestBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


