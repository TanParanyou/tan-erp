//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_response.g.dart';

/// QuotationResponse
///
/// Properties:
/// * [quotationId] 
/// * [estimateId] 
/// * [opportunityId] 
/// * [number] 
/// * [status] 
/// * [grandTotal] 
/// * [issuedAtUtc] 
/// * [estimateRevisionId] 
/// * [revisionNo] 
/// * [opportunityStage] 
/// * [opportunityRowVersion] 
/// * [estimateRowVersion] 
@BuiltValue()
abstract class QuotationResponse implements Built<QuotationResponse, QuotationResponseBuilder> {
  @BuiltValueField(wireName: r'quotationId')
  String? get quotationId;

  @BuiltValueField(wireName: r'estimateId')
  String? get estimateId;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'grandTotal')
  double? get grandTotal;

  @BuiltValueField(wireName: r'issuedAtUtc')
  DateTime? get issuedAtUtc;

  @BuiltValueField(wireName: r'estimateRevisionId')
  String? get estimateRevisionId;

  @BuiltValueField(wireName: r'revisionNo')
  int? get revisionNo;

  @BuiltValueField(wireName: r'opportunityStage')
  String? get opportunityStage;

  @BuiltValueField(wireName: r'opportunityRowVersion')
  String? get opportunityRowVersion;

  @BuiltValueField(wireName: r'estimateRowVersion')
  String? get estimateRowVersion;

  QuotationResponse._();

  factory QuotationResponse([void updates(QuotationResponseBuilder b)]) = _$QuotationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationResponse> get serializer => _$QuotationResponseSerializer();
}

class _$QuotationResponseSerializer implements PrimitiveSerializer<QuotationResponse> {
  @override
  final Iterable<Type> types = const [QuotationResponse, _$QuotationResponse];

  @override
  final String wireName = r'QuotationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.quotationId != null) {
      yield r'quotationId';
      yield serializers.serialize(
        object.quotationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateId != null) {
      yield r'estimateId';
      yield serializers.serialize(
        object.estimateId,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.grandTotal != null) {
      yield r'grandTotal';
      yield serializers.serialize(
        object.grandTotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.issuedAtUtc != null) {
      yield r'issuedAtUtc';
      yield serializers.serialize(
        object.issuedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.estimateRevisionId != null) {
      yield r'estimateRevisionId';
      yield serializers.serialize(
        object.estimateRevisionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.revisionNo != null) {
      yield r'revisionNo';
      yield serializers.serialize(
        object.revisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.opportunityStage != null) {
      yield r'opportunityStage';
      yield serializers.serialize(
        object.opportunityStage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityRowVersion != null) {
      yield r'opportunityRowVersion';
      yield serializers.serialize(
        object.opportunityRowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateRowVersion != null) {
      yield r'estimateRowVersion';
      yield serializers.serialize(
        object.estimateRowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'quotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationId = valueDes;
          break;
        case r'estimateId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateId = valueDes;
          break;
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'grandTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grandTotal = valueDes;
          break;
        case r'issuedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.issuedAtUtc = valueDes;
          break;
        case r'estimateRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateRevisionId = valueDes;
          break;
        case r'revisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revisionNo = valueDes;
          break;
        case r'opportunityStage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityStage = valueDes;
          break;
        case r'opportunityRowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityRowVersion = valueDes;
          break;
        case r'estimateRowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateRowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationResponseBuilder();
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


