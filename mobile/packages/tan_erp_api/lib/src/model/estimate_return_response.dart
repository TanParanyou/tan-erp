//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/estimate_return_reviewer_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_return_response.g.dart';

/// EstimateReturnResponse
///
/// Properties:
/// * [reasonCode] 
/// * [note] 
/// * [returnedAtUtc] 
/// * [returnedBy] 
@BuiltValue()
abstract class EstimateReturnResponse implements Built<EstimateReturnResponse, EstimateReturnResponseBuilder> {
  @BuiltValueField(wireName: r'reasonCode')
  String get reasonCode;

  @BuiltValueField(wireName: r'note')
  String get note;

  @BuiltValueField(wireName: r'returnedAtUtc')
  DateTime get returnedAtUtc;

  @BuiltValueField(wireName: r'returnedBy')
  EstimateReturnReviewerResponse get returnedBy;

  EstimateReturnResponse._();

  factory EstimateReturnResponse([void updates(EstimateReturnResponseBuilder b)]) = _$EstimateReturnResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReturnResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReturnResponse> get serializer => _$EstimateReturnResponseSerializer();
}

class _$EstimateReturnResponseSerializer implements PrimitiveSerializer<EstimateReturnResponse> {
  @override
  final Iterable<Type> types = const [EstimateReturnResponse, _$EstimateReturnResponse];

  @override
  final String wireName = r'EstimateReturnResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReturnResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'reasonCode';
    yield serializers.serialize(
      object.reasonCode,
      specifiedType: const FullType(String),
    );
    yield r'note';
    yield serializers.serialize(
      object.note,
      specifiedType: const FullType(String),
    );
    yield r'returnedAtUtc';
    yield serializers.serialize(
      object.returnedAtUtc,
      specifiedType: const FullType(DateTime),
    );
    yield r'returnedBy';
    yield serializers.serialize(
      object.returnedBy,
      specifiedType: const FullType(EstimateReturnReviewerResponse),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReturnResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReturnResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.reasonCode = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.note = valueDes;
          break;
        case r'returnedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(DateTime),
          ) as DateTime;
          result.returnedAtUtc = valueDes;
          break;
        case r'returnedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(EstimateReturnReviewerResponse),
          ) as EstimateReturnReviewerResponse;
          result.returnedBy.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReturnResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReturnResponseBuilder();
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


