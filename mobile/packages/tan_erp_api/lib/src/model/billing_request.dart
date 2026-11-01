//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/billing_line_request.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_request.g.dart';

/// BillingRequest
///
/// Properties:
/// * [projectId] 
/// * [kind] 
/// * [description] 
/// * [amount] 
/// * [dueDate] 
/// * [applyVat] 
/// * [vatRate] 
/// * [retentionRate] 
/// * [lines] 
@BuiltValue()
abstract class BillingRequest implements Built<BillingRequest, BillingRequestBuilder> {
  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'dueDate')
  Date? get dueDate;

  @BuiltValueField(wireName: r'applyVat')
  bool? get applyVat;

  @BuiltValueField(wireName: r'vatRate')
  double? get vatRate;

  @BuiltValueField(wireName: r'retentionRate')
  double? get retentionRate;

  @BuiltValueField(wireName: r'lines')
  BuiltList<BillingLineRequest>? get lines;

  BillingRequest._();

  factory BillingRequest([void updates(BillingRequestBuilder b)]) = _$BillingRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingRequest> get serializer => _$BillingRequestSerializer();
}

class _$BillingRequestSerializer implements PrimitiveSerializer<BillingRequest> {
  @override
  final Iterable<Type> types = const [BillingRequest, _$BillingRequest];

  @override
  final String wireName = r'BillingRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.projectId != null) {
      yield r'projectId';
      yield serializers.serialize(
        object.projectId,
        specifiedType: const FullType(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.dueDate != null) {
      yield r'dueDate';
      yield serializers.serialize(
        object.dueDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.applyVat != null) {
      yield r'applyVat';
      yield serializers.serialize(
        object.applyVat,
        specifiedType: const FullType(bool),
      );
    }
    if (object.vatRate != null) {
      yield r'vatRate';
      yield serializers.serialize(
        object.vatRate,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.retentionRate != null) {
      yield r'retentionRate';
      yield serializers.serialize(
        object.retentionRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BillingLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'projectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectId = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'dueDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.dueDate = valueDes;
          break;
        case r'applyVat':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.applyVat = valueDes;
          break;
        case r'vatRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatRate = valueDes;
          break;
        case r'retentionRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.retentionRate = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BillingLineRequest)]),
          ) as BuiltList<BillingLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BillingRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingRequestBuilder();
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


