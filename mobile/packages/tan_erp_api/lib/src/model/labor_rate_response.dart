//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'labor_rate_response.g.dart';

/// LaborRateResponse
///
/// Properties:
/// * [id] 
/// * [worker] 
/// * [hourlyRate] 
/// * [updatedAtUtc] 
/// * [rowVersion] 
@BuiltValue()
abstract class LaborRateResponse implements Built<LaborRateResponse, LaborRateResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'worker')
  ProjectPersonResponse? get worker;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  LaborRateResponse._();

  factory LaborRateResponse([void updates(LaborRateResponseBuilder b)]) = _$LaborRateResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(LaborRateResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<LaborRateResponse> get serializer => _$LaborRateResponseSerializer();
}

class _$LaborRateResponseSerializer implements PrimitiveSerializer<LaborRateResponse> {
  @override
  final Iterable<Type> types = const [LaborRateResponse, _$LaborRateResponse];

  @override
  final String wireName = r'LaborRateResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    LaborRateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.worker != null) {
      yield r'worker';
      yield serializers.serialize(
        object.worker,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.hourlyRate != null) {
      yield r'hourlyRate';
      yield serializers.serialize(
        object.hourlyRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    LaborRateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required LaborRateResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'worker':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.worker.replace(valueDes);
          break;
        case r'hourlyRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.hourlyRate = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  LaborRateResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = LaborRateResponseBuilder();
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


